using MongoDB.Driver;

namespace OptimaJet.DataEngine.Mongo;

internal class SessionOptions
{
    public MongoClient? ExternalClient { get; set; }
    public string ConnectionString { get; set; } = String.Empty;
}