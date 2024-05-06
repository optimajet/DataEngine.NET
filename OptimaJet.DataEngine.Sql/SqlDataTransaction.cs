using System.Data;

namespace OptimaJet.DataEngine.Sql;

public class SqlDataTransaction : IDataTransaction
{
    public SqlDataTransaction(IDbTransaction transaction, SqlDatabase database, IsolationLevel isolationLevel, Action onCompleteFn)
    {
        _transaction = transaction;
        Database = database;
        IsolationLevel = isolationLevel;
        _onCompleteFn = onCompleteFn;
    }

    public IDatabase Database { get; }

    public IsolationLevel IsolationLevel { get; }

    public void Commit()
    {
        _transaction.Commit();
        _onCompleteFn();
    }

    public void Rollback()
    {
        _transaction.Rollback();
        _onCompleteFn();
    }

    public void Dispose()
    {
        if (_disposed) return;

        _transaction.Dispose();
        
        _disposed = true;
    }

    private bool _disposed;
    private readonly IDbTransaction _transaction;
    private readonly Action _onCompleteFn;
}