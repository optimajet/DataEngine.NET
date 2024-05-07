namespace OptimaJet.DataEngine;

internal class VirtualTransaction : ITransaction
{
    public VirtualTransaction(Func<Task>? onCommittedAsync, Func<Task>? onRolledBackAsync)
    {
        Status = TransactionStatus.Pending;
        _onCommittedAsync = onCommittedAsync;
        _onRolledBackAsync = onRolledBackAsync;
    }

    public TransactionStatus Status { get; private set; }

    public async Task CommitAsync()
    {
        if (Status != TransactionStatus.Pending)
        {
            throw new InvalidOperationException("Cannot commit a transaction that has already been committed or rolled back.");
        }

        Status = TransactionStatus.Committed;
        if (_onCommittedAsync != null) await _onCommittedAsync();
    }

    public async Task RollbackAsync()
    {
        if (Status != TransactionStatus.Pending)
        {
            throw new InvalidOperationException("Cannot rollback a transaction that has already been committed or rolled back.");
        }

        Status = TransactionStatus.RolledBack;
        if (_onRolledBackAsync != null) await _onRolledBackAsync();
    }

    private readonly Func<Task>? _onCommittedAsync;
    private readonly Func<Task>? _onRolledBackAsync;
    
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
        if (Status == TransactionStatus.Pending) await RollbackAsync();
    }

    #endregion
}