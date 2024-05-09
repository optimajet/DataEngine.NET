using System.Data;
using System.Text;
using Dapper;
using OptimaJet.DataEngine.Exceptions;
using OptimaJet.DataEngine.Metadata;
using OptimaJet.DataEngine.Queries;
using SqlKata;
using SqlKata.Execution;

namespace OptimaJet.DataEngine.Sql.Queries;

/// <summary>
/// Decorator for SQLKata query.
/// It is needed for the redistribution of responsibility,
/// because access to the transaction and connection should be only when creating a query.
/// Encapsulates a connection and a transaction.
/// </summary>
internal class DataQuery
{
    /// <param name="provider"></param>
    /// <param name="connection">Any ADO.NET database connection</param>
    /// <param name="transaction">Any ADO.NET transaction</param>
    public DataQuery(SqlProvider provider, IDbConnection connection, IDbTransaction? transaction = null)
    {
        _query = new XQuery(connection, provider.Compiler) { Logger = LogRawSql };
        _provider = provider;
        _connection = connection;
        _transaction = transaction;
    }

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

        return ExecuteWithExceptionHandling(async () => (await _query.GetAsync<TEntity>(_transaction, _provider.DefaultTimeout)).ToList());
    }
    
    public Task<TEntity?> FirstAsync<TEntity>(EntityMetadata metadata) where TEntity : class
    {
        foreach (var column in metadata.Columns)
        {
            _query.Select($"{column.Name} as {column.OriginalName}");
        }

        return ExecuteWithExceptionHandling(async () => await _query.FirstOrDefaultAsync<TEntity>(_transaction, _provider.DefaultTimeout))!;
    }
    
    public Task<int> CountAsync()
    {
        return ExecuteWithExceptionHandling(() => _query.CountAsync<int>(null, _transaction, _provider.DefaultTimeout));
    }

    public Task<int> InsertAsync(Dictionary<string, object?> data)
    {
        return ExecuteWithExceptionHandling(() => _query.InsertAsync(data, _transaction, _provider.DefaultTimeout));
    }

    public async Task<int> InsertAsync(IEnumerable<string> columns, IEnumerable<IEnumerable<object?>> valuesCollection)
    {
        var valuesList = valuesCollection.ToList();

        if (valuesList.Count == 0) return 0;
        
        return await ExecuteWithExceptionHandling(() => 
            _provider.Name != ProviderName.Oracle
                ? _query.InsertAsync(columns, valuesList, _transaction, _provider.DefaultTimeout)
                : InsertOracleAsync(columns.ToList(), valuesList.ToList()));
    }

    public Task<int> UpdateAsync(Dictionary<string, object?> data)
    {
        return ExecuteWithExceptionHandling(() =>_query.UpdateAsync(data, _transaction, _provider.DefaultTimeout));
    }

    public Task<int> DeleteAsync()
    {
        return ExecuteWithExceptionHandling(() => _query.DeleteAsync(_transaction, _provider.DefaultTimeout));
    }

    #endregion

    #region CustomExecutions

    public Task<int> UpsertAsync<TEntity>(TEntity entity, EntityMetadata metadata) where TEntity : class
    {
        return ExecuteWithExceptionHandling(() => 
            _provider.Name switch
            {
                ProviderName.Mssql => UpsertMssqlAsync(entity, metadata),
                ProviderName.Mysql => UpsertMysqlAsync(entity, metadata),
                ProviderName.Oracle => UpsertOracleAsync(entity, metadata),
                ProviderName.Postgres => UpsertPostgresAsync(entity, metadata),
                //Same dialect as Postgres
                ProviderName.Sqlite => UpsertPostgresAsync(entity, metadata),
                _ => throw new ProviderNotSupportedException(_provider.Name)
            });
    }

    #endregion

    private SqlKata.Query _query;
    private string? _from;
    private readonly SqlProvider _provider;
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

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

        return await _connection.ExecuteAsync(command, parameters, _transaction, _provider.DefaultTimeout);
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

        return _connection.ExecuteAsync(command, parameters, _transaction, _provider.DefaultTimeout);
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

        return await _connection.ExecuteAsync(command, parameters, _transaction, _provider.DefaultTimeout) > 0 ? 1 : 0;
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

        return await _connection.ExecuteAsync(command, parameters, _transaction, _provider.DefaultTimeout) == -1 ? 1 : 0;
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

        return _connection.ExecuteAsync(command, parameters, _transaction, _provider.DefaultTimeout);
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
            _provider.ExceptionHandler(exception);
            throw;
        }
    }
    
    private void LogRawSql(SqlResult sql)
    {
        _provider.LogQueryAction(sql.RawSql);
    }
}
