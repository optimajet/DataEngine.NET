using System.Data;
using MongoDB.Driver;

namespace OptimaJet.DataEngine.Mongo;

public class MongoTransaction : IDataTransaction
{
    public MongoTransaction(IDatabase database, IClientSessionHandle session, Action onCompleteFn)
    {
        Database = database;
        _session = session;
        _onCompleteFn = onCompleteFn;
        
        _session.StartTransaction();
    }
    
    public IDatabase Database { get; }
    public IsolationLevel IsolationLevel => IsolationLevel.ReadCommitted;
    
    public void Commit()
    {
        _session.CommitTransaction();
        _onCompleteFn();
    }

    public void Rollback()
    {
        _session.AbortTransaction();
        _onCompleteFn();
    }
    
    public void Dispose()
    {
        if (_disposed) return;

        _session.Dispose();
        
        _disposed = true;
    }

    private readonly IClientSessionHandle _session;
    private readonly Action _onCompleteFn;
    private bool _disposed;
}