using MongoDB.Driver;
using OptimaJet.DataEngine.Mongo.Implementation;

namespace OptimaJet.DataEngine.Mongo;

public class MongoProvider : IProvider
{
    public static ProviderContext Use(string connectionString)
    {
        return ProviderContext.Use(new MongoProviderBuilder(connectionString, false));
    }
    
    public static ProviderContext Use(string connectionString, MongoClient externalClient)
    {
        return ProviderContext.Use(new MongoProviderBuilder(connectionString, externalClient, false));
    }

    public static ProviderContext Create(string connectionString)
    {
        return ProviderContext.Use(new MongoProviderBuilder(connectionString, true));
    }

    public static ProviderContext Create(string connectionString, MongoClient externalClient)
    {
        return ProviderContext.Use(new MongoProviderBuilder(connectionString, externalClient, true));
    }
    
    internal MongoProvider(SessionOptions sessionOptions)
    {
        _options = sessionOptions;
    }
    
    public string Name => ProviderName.Mongo;
    
    public ISession Session
    {
        get
        {
            _session ??= new MongoSession(
                this,
                _options.ExternalClient ?? new MongoClient(_options.ConnectionString)
            );
            
            return _session;
        }
    }
    
    public ICollection<TEntity> GetCollection<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity);
        
        if (!_collections.ContainsKey(type))
        {
            _collections[type] = new MongoCollection<TEntity>(this);
        }
        
        return (ICollection<TEntity>) _collections[type];
    }

    public OptionsRestorer UseOptions(Action<MongoOptions> configureOptions)
    {
        var clone = Options.Clone();
        configureOptions(clone);
        return UseOptions(clone);
    }

    public OptionsRestorer UseOptions(MongoOptions options)
    {
        var restorer = new OptionsRestorer(Options, o => Options = (MongoOptions) o);
        Options = options;
        return restorer;
    }

    OptionsRestorer IProvider.UseOptions(Action<IOptions> configureOptions)
    {
        return UseOptions(configureOptions);
    }

    OptionsRestorer IProvider.UseOptions(IOptions options)
    {
        return UseOptions((MongoOptions) options);
    }

    public MongoOptions Options { get; private set; } = new();
    public string ConnectionString => _options.ConnectionString;
    
    private ISession? _session;
    private readonly SessionOptions _options;
    private readonly Dictionary<Type, object> _collections = new();
    
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
        if (_session != null)
        {
            await _session.DisposeAsync();
            _session = null;
        }
    }
    
    ~MongoProvider()
    {
        DisposeAsyncInternal().GetAwaiter().GetResult();
    }
    
    #endregion
}