namespace OptimaJet.DataEngine.Sql;

public abstract class SqlProviderBuilder : IProviderBuilder
{
    protected SqlProviderBuilder(ISqlImplementation implementation, SessionOptions sessionOptions,
        bool isUniqueInstance)
    {
        _implementation = implementation;
        _sessionOptions = sessionOptions;
        _isUniqueInstance = isUniqueInstance;
    }
    
    public ProviderKey GetKey()
    {
        return (_sessionOptions, _isUniqueInstance) switch
        {
            ({ExternalConnection: not null}, true)
                => ProviderKey.GetUniqueKey(_implementation.Name, _sessionOptions.ExternalConnection),
            ({ExternalConnection: not null}, false)
                => ProviderKey.GetKey(_implementation.Name, _sessionOptions.ExternalConnection),
            ({ConnectionString: not null}, true)
                => ProviderKey.GetUniqueKey(_implementation.Name, _sessionOptions.ConnectionString),
            ({ConnectionString: not null}, false)
                => ProviderKey.GetKey(_implementation.Name, _sessionOptions.ConnectionString),
            _ => throw new InvalidOperationException(
                "Invalid options. Either ConnectionString or ExternalClient must be set.")
        };
    }
    
    public IProvider Build()
    {
        return new SqlProvider(_implementation, _sessionOptions)
        {
            DefaultTimeout = (int) Math.Ceiling(_sessionOptions.CommandTimeout.TotalSeconds)
        };
    }
    
    
    private readonly ISqlImplementation _implementation;
    private readonly SessionOptions _sessionOptions;
    private readonly bool _isUniqueInstance;
}