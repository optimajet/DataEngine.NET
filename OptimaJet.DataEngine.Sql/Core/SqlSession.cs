using System.Data;
using System.Data.Common;
using OptimaJet.DataEngine.Exceptions;
using OptimaJet.DataEngine.Sql.SqlDataQuery;

namespace OptimaJet.DataEngine.Sql;

public class SqlSession : ISession
{
    public SqlSession(SqlProvider provider, DbConnection connection, bool disposeConnection)
    {
        _provider = provider;
        _connection = connection;
        _disposeConnection = disposeConnection;
        
        if (!_disposeConnection)
        {
            _connection.Disposed += (_, _) => DisposeAsync().AsTask().Wait();
        }
    }
    
    public IProvider Provider => _provider;
    public ITransaction? CurrentTransaction => _virtualTransactions.Count > 0 ? _virtualTransactions.Peek() : null;
    
    public async Task<ITransaction> BeginTransactionAsync()
    {
        await EnsureConnectionOpenAsync();
        
        if (_transaction == null)
        {
            _transaction = _connection?.BeginTransaction();
            _disposeTransaction = true;
        }
        
        return PushVirtualTransaction();
    }
    
    public async Task<ITransaction> AttachTransactionAsync(DbTransaction transaction, bool disposeTransaction = false)
    {
        await EnsureConnectionOpenAsync();
        
        if (_transaction != null)
        {
            throw new InvalidOperationException("There is already an active transaction.");
        }
        
        if (!transaction.Connection.Equals(_connection))
        {
            throw new ArgumentException(
                "Transaction must be created on the same connection as the session's connection.", nameof(transaction));
        }
        
        _transaction = transaction;
        _disposeTransaction = disposeTransaction;
        
        return PushVirtualTransaction();
    }
    
    internal async Task<DataQuery> CreateQueryAsync()
    {
        await EnsureConnectionOpenAsync();
        return new DataQuery(_provider, _connection!, _transaction);
    }
    
    private async Task EnsureConnectionOpenAsync()
    {
        if (_connection == null)
        {
            throw new SessionEndedException();
        }
        
        // If the transaction's connection is null, it indicates that the transaction has ended
        // and can no longer be used.
        // However, in some drivers (for example, Postgres and SQLite), an ObjectDisposedException
        // will be thrown in the same case.
        if (_transaction != null)
        {
            try
            {
                if (_transaction.Connection == null)
                {
                    ClearTransaction();
                }
            }
            catch (ObjectDisposedException)
            {
                ClearTransaction();
            }
        }
        
        if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync();
        }
    }
    
    private void ClearTransaction()
    {
        if (_virtualTransactions.Count > 1)
        {
            throw new SessionEndedException();
        }
        
        _transaction = null;
        _virtualTransactions.Clear();
    }
    
    private ITransaction PushVirtualTransaction()
    {
        var transaction = new VirtualTransaction(VirtualTransactionCommitHandler, VirtualTransactionRollbackHandler);
        _virtualTransactions.Push(transaction);
        return transaction;
    }
    
    private Task VirtualTransactionCommitHandler()
    {
        if (_virtualTransactions.Count == 0)
        {
            throw new InvalidOperationException("There are no transactions to commit.");
        }
        
        _virtualTransactions.Pop();
        
        if (_virtualTransactions.Count == 0)
        {
            _transaction?.Commit();
            _transaction = null;
        }
        
        return Task.CompletedTask;
    }
    
    private async Task VirtualTransactionRollbackHandler()
    {
        if (_virtualTransactions.Count == 0) return;
        
        _virtualTransactions.Pop();
        
        if (_virtualTransactions.Count != 0)
        {
            await _virtualTransactions.Peek().RollbackAsync();
        }
        else
        {
            _transaction?.Rollback();
            _transaction = null;
        }
    }
    
    private readonly SqlProvider _provider;
    private DbConnection? _connection;
    private DbTransaction? _transaction;
    private bool _disposeTransaction;
    private readonly bool _disposeConnection;
    private readonly Stack<VirtualTransaction> _virtualTransactions = new();
    
    #region IDisposable implementation
    
    public void Dispose()
    {
        DisposeAsync().AsTask().Wait();
    }
    
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncInternal();
        GC.SuppressFinalize(this);
    }
    
    private ValueTask DisposeAsyncInternal()
    {
        if (_transaction != null && _disposeTransaction)
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }
        
        _transaction = null;
        
        if (_connection != null && _disposeConnection)
        {
            _connection.Close();
            _connection.Dispose();
        }
        
        _connection = null;
        
        return default;
    }
    
    #endregion
}