using System.Data;

namespace OptimaJet.DataEngine;

/// <summary>
/// An interface that allows you to work with aspects
/// that are common to the entire database, rather than individual entities.
/// Encapsulates how a data provider's database is managed.
/// </summary>
public interface IDatabase : IDisposable
{
    /// <summary>
    /// Returns the type of the data provider
    /// </summary>
    ProviderType ProviderType { get; }
    
    /// <summary>
    /// Returns the current transaction in which all commands are executing
    /// </summary>
    IDataTransaction? CurrentTransaction { get; }

    /// <summary>
    /// Creates a new transaction, all further commands will be executed in this transaction.
    /// Also, the transaction is placed in the CurrentTransaction.
    /// Nested transactions are not currently supported.
    /// </summary>
    /// <param name="isolationLevel">Transaction isolation level</param>
    /// <returns>
    /// An object that allows you to manage a transaction.
    /// It can be used to commit or rollback. Requires disposing.
    /// </returns>
    Task<IDataTransaction> BeginTransactionAsync(IsolationLevel? isolationLevel = null);
    
    /// <summary>
    /// Creates a new transaction if not exist, all further commands will be executed in this transaction.
    /// Also, the transaction is placed in the CurrentTransaction.
    /// Nested transactions are not currently supported.
    /// </summary>
    /// <param name="isolationLevel">Transaction isolation level</param>
    /// <returns>
    /// An object that allows you to manage a transaction or null if transaction already exist.
    /// It can be used to commit or rollback. Requires disposing.
    /// </returns>
    Task<IDataTransaction?> BeginNestedTransactionAsync(IsolationLevel? isolationLevel = null);

    Task<IEnumerable<TResult>> ExecuteTableFunctionAsync<TEntity, TResult>(DataFunction<TEntity, TResult> function)
        where TEntity : class;
    
    Task<TResult?> ExecuteFunctionAsync<TEntity, TResult>(DataFunction<TEntity, TResult> function) 
        where TEntity : class;
}
