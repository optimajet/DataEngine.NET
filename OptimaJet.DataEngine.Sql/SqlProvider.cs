using OptimaJet.DataEngine.Metadata;
using OptimaJet.DataEngine.Sql.Implementation;
using SqlKata.Compilers;

namespace OptimaJet.DataEngine.Sql;

public class SqlProvider : IProvider
{
    internal SqlProvider(ISqlImplementation implementation, SessionOptions sessionOptions)
    {
        _implementation = implementation;
        _sessionOptions = sessionOptions;
    }

    public string Name => _implementation.Name;

    public ISession Session
    {
        get
        {
            _session ??= new SqlSession(
                this,
                _sessionOptions.ExternalConnection ??
                _implementation.CreateConnection(_sessionOptions.ConnectionString),
                _sessionOptions.ExternalConnection == null
            );

            return _session;
        }
    }

    public ICollection<TEntity> GetCollection<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity);

        if (!_collections.ContainsKey(type))
        {
            _implementation.ConfigureMetadata(MetadataPool<TEntity>.GetMetadata(Name));
            _collections[type] = new SqlCollection<TEntity>(this);
        }

        return (ICollection<TEntity>) _collections[type];
    }

    public Compiler Compiler => _implementation.Compiler;
    public Dialect Dialect => _implementation.Dialect;
    public string ConnectionString => _sessionOptions.ConnectionString;
    public int DefaultTimeout { get; set; } = 30;
    public Action<Exception> ExceptionHandler { get; set; } = _ => { };
    public Action<string> LogQueryAction { get; set; } = _ => { };

    private ISession? _session;
    private readonly ISqlImplementation _implementation;
    private readonly SessionOptions _sessionOptions;
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
    #endregion
}