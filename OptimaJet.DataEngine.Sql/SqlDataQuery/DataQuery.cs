using System.Data;
using System.Text;
using Dapper;
using Dapper.Oracle;
using OptimaJet.DataEngine.Exceptions;
using OptimaJet.DataEngine.Filters;
using OptimaJet.DataEngine.Metadata;
using OptimaJet.DataEngine.Sorts;
using OptimaJet.DataEngine.Sql.Glossaries;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace OptimaJet.DataEngine.Sql.SqlDataQuery;

/// <summary>
/// Decorator for SQLKata query.
/// It is needed for the redistribution of responsibility,
/// because access to the transaction and connection should be only when creating a query.
/// Encapsulates a connection and a transaction.
/// </summary>
public class DataQuery
{
    /// <param name="connection">Any ADO.NET database connection</param>
    /// <param name="dataProvider"></param>
    /// <param name="compiler">Specifies sql dialect Compiler </param>
    /// <param name="exceptionHandler">Client side exception handling</param>
    /// <param name="transaction">Any ADO.NET transaction</param>
    /// <param name="timeout">Command execution timeout on null using DefaultTimeout</param>
    public DataQuery(IDbConnection connection, ProviderType dataProvider, Compiler compiler, Func<Exception, Exception> exceptionHandler, IDbTransaction? transaction = null, int? timeout = null)
    {
        _query = new XQuery(connection, compiler) { Logger = LogRawSql };
        _connection = connection;
        _dataProvider = dataProvider;
        _transaction = transaction;
        _exceptionHandler = exceptionHandler;
        
        Timeout = timeout ?? DefaultTimeout;
    }

    public const int DefaultTimeout = 30;

    /// <summary>
    /// Command execution timeout
    /// </summary>
    public int Timeout { get; set; }
    
    public Action<string>? LogQueryFn { get; set; }

    #region CustomQueries

    public DataQuery BuildByConstructor<TEntity>(SelectConstructor<TEntity> constructor, EntityMetadata metadata) where TEntity : class
    {
        Where(constructor.Filter, metadata);
        Sort(constructor.Sort, metadata);
        Limit(constructor.Limit);
        Offset(constructor.Offset);

        return this;
    }

    public DataQuery Where(IFilter? filter, EntityMetadata metadata)
    {
        if (filter == null) return this;
        _query = _query.Where(_ => SqlFilterBuilder.Create(metadata).Build(filter));
        return this;
    }

    public DataQuery Sort(Sort? sort, EntityMetadata metadata)
    {
        if (sort == null) return this;

        foreach (var order in sort.Orders)
        {
            _query = order.Direction switch
            {
                Direction.Asc => _query.OrderBy(metadata.GetColumnNameByProperty(order.OriginalName)),
                Direction.Desc => _query.OrderByDesc(metadata.GetColumnNameByProperty(order.OriginalName)),
                _ => throw new OrderDirectionNotSupportedException(order.Direction.ToString())
            };
        }

        return this;
    }

    #endregion

    #region Query

    public DataQuery From(string tableName)
    {
        _from = tableName;
        _query = _query.From(_from);
        return this;
    }

    public DataQuery Where(string column, object? value)
    {
        _query = _query.Where(column, value);
        return this;
    }

    public DataQuery WhereIn<TValue>(string column, IEnumerable<TValue> values)
    {
        _query = _query.WhereIn(column, values);
        return this;
    }

    public DataQuery Limit(int? value)
    {
        if (value == null) return this;
        _query = _query.Limit((int) value);
        return this;
    }

    public DataQuery Offset(int? value)
    {
        if (value == null) return this;
        _query = _query.Offset((int) value);
        return this;
    }

    #endregion

    #region QueryExtentions

    public Task<List<TEntity>> GetAsync<TEntity>(EntityMetadata metadata) where TEntity : class
    {
        foreach (var column in metadata.Columns)
        {
            _query.Select($"{column.Name} as {column.OriginalName}");
        }

        return ExecuteWithExceptionHandling(async () => (await _query.GetAsync<TEntity>(_transaction, Timeout)).ToList());
    }
    
    public Task<TEntity?> FirstAsync<TEntity>(EntityMetadata metadata) where TEntity : class
    {
        foreach (var column in metadata.Columns)
        {
            _query.Select($"{column.Name} as {column.OriginalName}");
        }

        return ExecuteWithExceptionHandling(async () => await _query.FirstOrDefaultAsync<TEntity>(_transaction, Timeout))!;
    }
    
    public Task<int> CountAsync()
    {
        return ExecuteWithExceptionHandling(() => _query.CountAsync<int>(null, _transaction, Timeout));
    }

    public Task<int> InsertAsync(Dictionary<string, object?> data)
    {
        return ExecuteWithExceptionHandling(() => _query.InsertAsync(data, _transaction, Timeout));
    }

    public async Task<int> InsertAsync(IEnumerable<string> columns, IEnumerable<IEnumerable<object?>> valuesCollection)
    {
        var valuesList = valuesCollection.ToList();

        if (valuesList.Count == 0) return 0;
        
        return await ExecuteWithExceptionHandling(() => 
            _dataProvider != ProviderType.Oracle 
                ? _query.InsertAsync(columns, valuesList, _transaction, Timeout) 
                : InsertOracleAsync(columns.ToList(), valuesList.ToList()));
    }

    public Task<int> UpdateAsync(Dictionary<string, object?> data)
    {
        return ExecuteWithExceptionHandling(() =>_query.UpdateAsync(data, _transaction, Timeout));
    }

    public Task<int> DeleteAsync()
    {
        return ExecuteWithExceptionHandling(() => _query.DeleteAsync(_transaction, Timeout));
    }

    #endregion

    #region CustomExecutions

    public async Task<IEnumerable<TResult>> ExecuteStoredProcedureAsync<TEntity, TResult>(DataFunction<TEntity, TResult> function)
        where TEntity : class
    {
        const string oracleResult = "Result";

        var glossary = Dialect.Get(_dataProvider);
        var command = glossary.Quote(function.Name);

        if (_dataProvider != ProviderType.Oracle)
            return await _connection.QueryAsync<TResult>(command, function.Parameter, _transaction, Timeout,
                CommandType.StoredProcedure);

        var oracleParams = new OracleDynamicParameters();

        oracleParams.AddDynamicParams(function.Parameter);
        oracleParams.Add(oracleResult, dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

        return await ExecuteWithExceptionHandling(() => 
            _connection.QueryAsync<TResult>(command, oracleParams, _transaction, Timeout, CommandType.StoredProcedure));
    }

    public Task<int> UpsertAsync<TEntity>(TEntity entity, EntityMetadata metadata) where TEntity : class
    {
        return ExecuteWithExceptionHandling(() => 
            _dataProvider switch
            {
                ProviderType.Mssql => UpsertMssqlAsync(entity, metadata),
                ProviderType.Mysql => UpsertMysqlAsync(entity, metadata),
                ProviderType.Oracle => UpsertOracleAsync(entity, metadata),
                ProviderType.Postgres => UpsertPostgresAsync(entity, metadata),
                //Same dialect as Postgres
                ProviderType.Sqlite => UpsertPostgresAsync(entity, metadata),
                _ => throw new ProviderTypeNotSupportedException(_dataProvider)
            });
    }

    #endregion

    private Query _query;
    private string? _from;
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;
    private readonly Func<Exception, Exception> _exceptionHandler;
    private readonly ProviderType _dataProvider;

    #region TemporaryWorkarounds

    private async Task<int> InsertOracleAsync(List<string> columns, List<IEnumerable<object?>> valuesCollection)
    {
        if (valuesCollection.Count == 0) return 0;

        var parameters = new List<KeyValuePair<string, object?>>();
        var rowNumber = valuesCollection.Count;
        var rowLength = valuesCollection.First().Count();

        var sb = new StringBuilder();

        sb.Append("INTO \"");
        sb.Append(_from?.Replace(".", "\".\""));
        sb.Append("\" (");
        sb.Append(String.Join(", ", columns.Select(c => $"\"{c}\"")));
        sb.Append(") VALUES ");

        var intoText = sb.ToString();

        sb.Clear();
        sb.Append("INSERT ALL");
        sb.AppendLine();

        for (var i = 0; i < rowNumber; i++)
        {
            sb.Append(intoText);
            sb.Append("(");

            for (int j = 0; j < rowLength; j++)
            {
                //Filling in the command text
                var parameterName = $":p{rowLength * i + j}";

                sb.Append(parameterName);
                if (j < rowLength - 1) sb.Append(", ");

                //Filling in the parameter value collection
                var row = valuesCollection[i].ToList();
                parameters.Add(new KeyValuePair<string, object?>(parameterName, row[j]));
            }

            sb.Append(")");
            sb.AppendLine();
        }

        sb.Append($"SELECT {rowNumber} FROM DUAL");

        var command = sb.ToString();

        return await _connection.ExecuteAsync(command, parameters, _transaction, Timeout);
    }

    private Task<int> UpsertMssqlAsync<TEntity>(TEntity entity, EntityMetadata metadata) where TEntity : class
    {
        var parameters = CreateParameters(entity, metadata);
        var sb = new StringBuilder();

        sb.Append("BEGIN TRANSACTION; ");
        sb.Append($"UPDATE [{_from?.Replace(".", "].[")}] WITH (UPDLOCK, SERIALIZABLE) SET ");
        sb.Append(String.Join(",", metadata.NotPkColumnNames.Select(c => $"[{c}] = @{c}")));
        sb.Append($" WHERE ");
        sb.Append($"[{metadata.PrimaryKeyColumn.Name}] = @{metadata.PrimaryKeyColumn.Name}; ");
        sb.Append($"IF @@ROWCOUNT = 0 ");
        sb.Append($"BEGIN ");
        sb.Append($"INSERT INTO [{_from?.Replace(".", "].[")}] (");
        sb.Append(String.Join(",", metadata.ColumnNames.Select(c => $"[{c}]")));
        sb.Append($") VALUES (");
        sb.Append(String.Join(",", parameters.Keys));
        sb.Append($") ");
        sb.Append($"END; ");
        sb.Append($"COMMIT TRANSACTION;");

        var command = sb.ToString();

        return _connection.ExecuteAsync(command, parameters, _transaction, Timeout);
    }

    private async Task<int> UpsertMysqlAsync<TEntity>(TEntity entity, EntityMetadata metadata) where TEntity : class
    {
        var parameters = CreateParameters(entity, metadata);
        var sb = new StringBuilder();

        sb.Append($"INSERT INTO `{_from}` (");
        sb.Append(String.Join(",", metadata.ColumnNames.Select(c => $"`{c}`")));
        sb.Append($") VALUES (");
        sb.Append(String.Join(",", parameters.Keys));
        sb.Append($") ON DUPLICATE KEY UPDATE ");
        sb.Append(String.Join(",", metadata.NotPkColumnNames.Select(c => $"`{c}` = @{c}")));

        var command = sb.ToString();

        return await _connection.ExecuteAsync(command, parameters, _transaction, Timeout) > 0 ? 1 : 0;
    }

    private async Task<int> UpsertOracleAsync<TEntity>(TEntity entity, EntityMetadata metadata) where TEntity : class
    {
        var parameters = CreateParameters(entity, metadata, ":");
        var sb = new StringBuilder();

        sb.Append("BEGIN ");
        sb.Append("LOOP BEGIN ");
        sb.Append($"MERGE INTO {_from?.Replace(".", "\".\"")} ");
        sb.Append($"USING dual ON ");
        sb.Append($"(\"{metadata.PrimaryKeyColumn.Name}\" = :{metadata.PrimaryKeyColumn.Name}) ");
        sb.Append("WHEN NOT MATCHED THEN ");
        sb.Append($"INSERT (");
        sb.Append(String.Join(",", metadata.ColumnNames.Select(c => $"\"{c}\"")));
        sb.Append(") VALUES (");
        sb.Append(String.Join(",", parameters.Keys));
        sb.Append(") WHEN MATCHED THEN ");
        sb.Append("UPDATE SET ");
        sb.Append(String.Join(",", metadata.NotPkColumnNames.Select(c => $"\"{c}\" = :{c}")));
        sb.Append("; EXIT; -- success? -> exit loop ");
        sb.AppendLine();
        sb.Append("EXCEPTION ");
        sb.Append("WHEN NO_DATA_FOUND THEN ");
        sb.Append("NULL; -- exception? -> no op, i.e. continue looping ");
        sb.AppendLine();
        sb.Append("WHEN DUP_VAL_ON_INDEX THEN ");
        sb.Append("NULL; -- exception? -> no op, i.e. continue looping ");
        sb.AppendLine();
        sb.Append("END; ");
        sb.Append("END LOOP; ");
        sb.Append("END;");

        var command = sb.ToString();

        return await _connection.ExecuteAsync(command, parameters, _transaction, Timeout) == -1 ? 1 : 0;
    }

    private Task<int> UpsertPostgresAsync<TEntity>(TEntity entity, EntityMetadata metadata) where TEntity : class
    {
        var parameters = CreateParameters(entity, metadata);
        var sb = new StringBuilder();

        sb.Append($"INSERT INTO \"{_from?.Replace(".", "\".\"")}\" (");
        sb.Append(String.Join(",", metadata.ColumnNames.Select(c => $"\"{c}\"")));
        sb.Append($") VALUES (");
        sb.Append(String.Join(",", parameters.Keys));
        sb.Append($") ON CONFLICT (\"{metadata.PrimaryKeyColumn.Name}\") ");
        sb.Append($"DO UPDATE SET ");
        sb.Append(String.Join(",", metadata.NotPkColumnNames.Select(c => $"\"{c}\" = @{c}")));

        var command = sb.ToString();

        return _connection.ExecuteAsync(command, parameters, _transaction, Timeout);
    }

    private Dictionary<string, object?> CreateParameters<TEntity>(TEntity entity, EntityMetadata metadata, string prefix = "@") where TEntity : class
    {
        var result = new Dictionary<string, object?>();

        foreach (var column in metadata.Columns)
        {
            result[prefix + column.Name] = column.GetValue(entity);
        }

        return result;
    }

    #endregion

    private async Task<TResult> ExecuteWithExceptionHandling<TResult>(Func<Task<TResult>> func)
    {
        try
        {
            return await func();
        }
        catch (Exception exception)
        {
            throw _exceptionHandler(exception);
        }
    }
    
    private void LogRawSql(SqlResult sql)
    {
        if (LogQueryFn != null && sql.RawSql != null)
        {
            LogQueryFn(sql.RawSql);
        }
    }
}
