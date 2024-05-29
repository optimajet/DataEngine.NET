using MongoDB.Driver;
using OptimaJet.DataEngine.Mongo.Implementation;

namespace OptimaJet.DataEngine.Mongo;

public class MongoProviderBuilder : IProviderBuilder
{
    public MongoProviderBuilder(string connectionString, bool isUniqueInstance)
    {
        _sessionOptions.ConnectionString = connectionString;
        _isUniqueInstance = isUniqueInstance;
    }
    
    public MongoProviderBuilder(string connectionString, MongoClient externalClient, bool isUniqueInstance)
    {
        _sessionOptions.ConnectionString = connectionString;
        _sessionOptions.ExternalClient = externalClient;
        _isUniqueInstance = isUniqueInstance;
    }
    
    public ProviderKey GetKey()
    {
        if (_isUniqueInstance)
        {
            return ProviderKey.GetUniqueKey(ProviderName.Mongo);
        }

        if (_sessionOptions.ExternalClient != null)
        {
            return ProviderKey.GetKey(ProviderName.Mongo, _sessionOptions.ExternalClient);
        }

        return ProviderKey.GetKey(ProviderName.Mongo, _sessionOptions.ConnectionString);
    }

    public IProvider Build()
    {
        return new MongoProvider(_sessionOptions);
    }
    
    private readonly SessionOptions _sessionOptions = new();
    private readonly bool _isUniqueInstance;
}