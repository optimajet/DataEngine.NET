using MongoDB.Driver;
using OptimaJet.DataEngine.Mongo.Implementation;

namespace OptimaJet.DataEngine.Mongo;

public class MongoProviderBuilder : IProviderBuilder
{
    public MongoProviderBuilder(string connectionString, bool isUniqueInstance)
    {
        _isUniqueInstance = isUniqueInstance;
        _sessionOptions.ConnectionString = connectionString;
    }
    
    public MongoProviderBuilder(string connectionString, MongoClient externalClient, bool isUniqueInstance)
    {
        _sessionOptions.ConnectionString = connectionString;
        _sessionOptions.ExternalClient = externalClient;
    }
    
    public ProviderKey GetKey()
    {
        return (_sessionOptions, _isUniqueInstance) switch
        {
            ({ExternalClient: not null}, true)
                => ProviderKey.GetUniqueKey(ProviderName.Mongo, _sessionOptions.ExternalClient),
            ({ExternalClient: not null}, false)
                => ProviderKey.GetKey(ProviderName.Mongo, _sessionOptions.ExternalClient),
            ({ConnectionString: not null}, true)
                => ProviderKey.GetUniqueKey(ProviderName.Mongo, _sessionOptions.ConnectionString),
            ({ConnectionString: not null}, false)
                => ProviderKey.GetKey(ProviderName.Mongo, _sessionOptions.ConnectionString),
            _ => throw new InvalidOperationException(
                "Invalid options. Either ConnectionString or ExternalClient must be set.")
        };
    }
    
    public IProvider Build()
    {
        return new MongoProvider(_sessionOptions);
    }
    
    private readonly SessionOptions _sessionOptions = new();
    private readonly bool _isUniqueInstance;
}