namespace OptimaJet.DataEngine;

public interface ITransaction : IAsyncDisposable, IDisposable
{
    public TransactionStatus Status { get; }
    public Task CommitAsync();
    public Task RollbackAsync();
}