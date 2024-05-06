using MongoDB.Driver;
using OptimaJet.DataEngine.Exceptions;

namespace OptimaJet.DataEngine.Mongo;

public class MongoDataFactory : IConfigurableDataFactory
{
    public MongoDataFactory(string connectionString) 
        : this(new DataFactoryOptions(connectionString), new MongoClient(connectionString)) 
    {}
    
    public MongoDataFactory(DataFactoryOptions options) 
        : this(options, new MongoClient(options.DatabaseOptions.ConnectionString)) 
    {}

    public MongoDataFactory(DataFactoryOptions options, MongoClient client)
    {
        Options = options;
        MongoClient = client;
    }

    public DataFactoryOptions Options { get; set; }

    public IDatabase CreateDatabase()
    {
        _database = new MongoDatabase(Options.DatabaseOptions, MongoClient);
        return _database;
    }

    public IDataSet<TEntity> CreateDataSet<TEntity>() where TEntity : class
    {
        if (_database == null) throw new MissingDatabaseException();
        
        return new MongoDataSet<TEntity>(_database, Options.DataSetOptions);
    }

    protected MongoClient MongoClient { get; }
    private MongoDatabase? _database;
}