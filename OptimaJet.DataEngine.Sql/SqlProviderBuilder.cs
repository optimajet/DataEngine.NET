using System.Data.Common;
using OptimaJet.DataEngine.Sql.Implementation;

namespace OptimaJet.DataEngine.Sql;

public abstract class SqlProviderBuilder : IProviderBuilder
{
    internal SqlProviderBuilder(ISqlImplementation implementation, string connectionString, bool isUniqueInstance)
    {
        _implementation = implementation;
        _sessionOptions = new SessionOptions(connectionString);
        _isUniqueInstance = isUniqueInstance;
    }

    internal SqlProviderBuilder(ISqlImplementation implementation, DbConnection connection, bool isUniqueInstance)
    {
        _implementation = implementation;
        _sessionOptions = new SessionOptions(connection);
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
        return new SqlProvider(_implementation, _sessionOptions);
    }
    
    
    private readonly ISqlImplementation _implementation;
    private readonly SessionOptions _sessionOptions;
    private readonly bool _isUniqueInstance;
}