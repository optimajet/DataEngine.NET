using System.Data;

namespace OptimaJet.DataEngine;

/// <summary>
/// An interface that allows you to work with a transaction object.
/// Encapsulates transaction logic.
/// </summary>
public interface IDataTransaction : IDisposable
{
    /// <summary>
    /// The database in which the transaction was created
    /// </summary>
    IDatabase Database { get; }
    
    /// <summary>
    /// Isolation level of transaction
    /// </summary>
    IsolationLevel IsolationLevel { get; }
    
    /// <summary>
    /// Commit transaction and save changes in database
    /// </summary>
    void Commit();
    
    /// <summary>
    /// Rollback all uncommitted changes
    /// </summary>
    void Rollback();
}
