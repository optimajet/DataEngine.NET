namespace OptimaJet.DataEngine;

public interface ISession : IAsyncDisposable, IDisposable
{
    IProvider Provider { get; }
    ITransaction? CurrentTransaction { get; }
    Task<ITransaction> BeginTransactionAsync();
}