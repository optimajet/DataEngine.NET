using MongoDB.Driver;
using OptimaJet.DataEngine.Helpers;

namespace OptimaJet.DataEngine.Mongo.Implementation;

internal class MongoSession : ISession
{
    public MongoSession(MongoProvider provider, MongoClient client)
    {
        _provider = provider;
        _client = client;
        _database = client.GetDatabase(new MongoUrl(provider.ConnectionString).DatabaseName);
    }

    public IProvider Provider => _provider;
    public ITransaction? CurrentTransaction => _virtualTransactions.Count > 0 ? _virtualTransactions.Peek() : null;

    public IMongoCollection<TEntity> GetCollection<TEntity>(string name) where TEntity : class
    {
        return _database.GetCollection<TEntity>(name);
    }

    public async Task<ITransaction> BeginTransactionAsync()
    {
        if (_session == null)
        {
            _session = await _client.StartSessionAsync();
            _disposeSession = true;
        }

        if (!_session.IsInTransaction) _session.StartTransaction();

        return PushVirtualTransaction();
    }

    public Task<ITransaction> AttachSessionAsync(IClientSessionHandle session, bool disposeSession = false)
    {
        if (_session != null)
        {
            throw new InvalidOperationException("There is already an active session.");
        }

        if (!session.Client.Equals(_client))
        {
            throw new ArgumentException("Session must be created on the same client as the session's client.", nameof(session));
        }

        _session = session;
        _disposeSession = disposeSession;

        return Task.FromResult(PushVirtualTransaction());
    }

    private ITransaction PushVirtualTransaction()
    {
        var transaction = new VirtualTransaction(VirtualTransactionCommitHandler, VirtualTransactionRollbackHandler);
        _virtualTransactions.Push(transaction);
        return transaction;
    }

    private async Task VirtualTransactionCommitHandler()
    {
        if (_virtualTransactions.Count == 0)
        {
            throw new InvalidOperationException("There are no transactions to commit.");
        }

        _virtualTransactions.Pop();

        if (_virtualTransactions.Count == 0 && _session != null)
        {
           await _session.CommitTransactionAsync();
        }
    }

    private async Task VirtualTransactionRollbackHandler()
    {
        if (_virtualTransactions.Count == 0) return;

        _virtualTransactions.Pop();

        if (_virtualTransactions.Count == 0 && _session != null)
        {
            await _session.AbortTransactionAsync();
        }
        else
        {
            await _virtualTransactions.Peek().RollbackAsync();
        }
    }

    private readonly MongoProvider _provider;
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;
    private IClientSessionHandle? _session;
    private bool _disposeSession= true;
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

    private async ValueTask DisposeAsyncInternal()
    {
        if (_session != null && _disposeSession)
        {
            if (_session.IsInTransaction)
            {
                await _session.AbortTransactionAsync();
            }
            _session.Dispose();
            _session = null;
        }
    }

    #endregion
}