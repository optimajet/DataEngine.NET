using System.Data;
using System.Data.Common;
using OptimaJet.DataEngine.Exceptions;
using OptimaJet.DataEngine.Sql.SqlDataQuery;
using SqlKata.Compilers;

namespace OptimaJet.DataEngine.Sql;

/// <summary>
/// An abstract base class that implements the common logic of all relative database providers.
/// </summary>
public abstract class SqlDatabase : IDatabase
{
    /// <summary>
    /// Creates the database object for manage database processes.
    /// Can be created using an existing connection and transaction to pick up the external transaction.
    /// </summary>
    /// <param name="options">Options for configuring database processes</param>
    /// <param name="compiler">Specifies sql dialect Compiler</param>
    /// <param name="connection">Any ADO.NET database connection</param>
    /// <param name="transaction">
    /// Allows you to specify an external transaction
    /// to pick it up and use it for further queries.
    /// </param>
    protected SqlDatabase(DatabaseOptions options, Compiler compiler, DbConnection connection, DbTransaction? transaction = null)
    {
        Options = options;
        Connection = connection;
        Compiler = compiler;

        if (transaction != null)
        {
            CurrentDbTransaction = transaction;
            CurrentTransaction = new SqlDataTransaction(transaction, this, transaction.IsolationLevel, CompleteTransaction);
        }
    }

    public abstract ProviderType ProviderType { get; }
    public bool IsConnectionOpen => ConnectionState == ConnectionState.Open;
    public ConnectionState ConnectionState => Connection.State;
    public IDataTransaction? CurrentTransaction { get; private set; }
    
    /// <summary>
    /// Returns the current connection string
    /// </summary>
    public string ConnectionString => Options.ConnectionString;
    
    /// <summary>
    /// Returns the current connection in opened state
    /// </summary>
    /// <returns>Current connection</returns>
    public async Task<DbConnection> GetConnectionAsync()
    {
        await OpenConnectionAsync();
        return Connection;
    }
    
    public async Task OpenConnectionAsync()
    {
        if (!IsConnectionOpen) await Connection.OpenAsync(); 
    }

    public async Task<IDataTransaction> BeginTransactionAsync(IsolationLevel? isolationLevel = null)
    {
        await OpenConnectionAsync();
        
        if (CurrentTransaction != null) throw new TransactionAlreadyExistException();
        
        var iso = isolationLevel ?? IsolationLevel.ReadCommitted;
        
        CurrentDbTransaction = Connection.BeginTransaction(iso);

        CurrentTransaction = new SqlDataTransaction(CurrentDbTransaction, this, iso, CompleteTransaction);

        return CurrentTransaction;
    }

    public async Task<IDataTransaction?> BeginNestedTransactionAsync(IsolationLevel? isolationLevel = null)
    {
        return CurrentTransaction != null 
            ? null 
            : await BeginTransactionAsync(isolationLevel);
    }
    
    public async Task<IEnumerable<TResult>> ExecuteTableFunctionAsync<TEntity, TResult>(DataFunction<TEntity, TResult> function)
        where TEntity : class
    {
        return await (await CreateQueryAsync()).ExecuteStoredProcedureAsync(function);
    }

    public async Task<TResult?> ExecuteFunctionAsync<TEntity, TResult>(DataFunction<TEntity, TResult> function)
        where TEntity : class
    {
        return (await ExecuteTableFunctionAsync(function)).FirstOrDefault();
    }
    

    /// <summary>
    /// Creates a DataQuery object with passing into it the current transaction if there,
    /// and the connection to the database.
    /// </summary>
    /// <param name="timeout">Command execution timeout, on null using GlobalCommandTimeout option</param>
    /// <returns>DataQuery object for creating and executing queries</returns>
    public async Task<DataQuery> CreateQueryAsync(int? timeout = null)
    {
        await OpenConnectionAsync();
        
        return new DataQuery(Connection, ProviderType, Compiler, Options.QueryExceptionHandler, CurrentDbTransaction, 
            timeout ?? Options.GlobalDefaultTimeout)
        {
            LogQueryFn = Options.LogQueryFn
        };
    }

    public void Dispose()
    {
        if (_disposed) return;

        CurrentTransaction?.Dispose();
        Connection.Dispose();
        _disposed = true;
    }
    
    protected DatabaseOptions Options { get; }
    protected Compiler Compiler { get; }
    protected DbConnection Connection { get; }
    protected DbTransaction? CurrentDbTransaction { get; private set; }

    private bool _disposed;
    
    private void CompleteTransaction()
    {
        CurrentDbTransaction = null;
        CurrentTransaction = null;
    }
}