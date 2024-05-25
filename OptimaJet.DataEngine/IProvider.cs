namespace OptimaJet.DataEngine;

public interface IProvider : IAsyncDisposable, IDisposable
{
    public string Name { get; }
    public ISession Session { get; }
    ICollection<TEntity> GetCollection<TEntity>() where TEntity : class;
    OptionsRestorer UseOptions(Action<IOptions> configureOptions);
    OptionsRestorer UseOptions(IOptions options);
}