using System.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using OptimaJet.DataEngine.Exceptions;

namespace OptimaJet.DataEngine.Mongo;

public class MongoDatabase : IDatabase
{
    static MongoDatabase()
    {
        var conventions = new ConventionPack
        {
            new IgnoreExtraElementsConvention(true)
        };
        
        ConventionRegistry.Register("DataEngine", conventions, _ => true);
        
        BsonSerializer.RegisterSerializer(typeof(DateTime), new DateTimeSerializer(DateTimeKind.Local));
        BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
        BsonSerializer.RegisterSerializer(typeof(decimal?), new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));
    }
    
    public MongoDatabase(DatabaseOptions options, MongoClient client)
    {
        Options = options;
        MongoClient = client;
        DatabaseName = MongoUrl.Create(Options.ConnectionString).DatabaseName;
        Store = MongoClient.GetDatabase(DatabaseName);
    }

    public ProviderType ProviderType => ProviderType.Mongo;
    public IDataTransaction? CurrentTransaction { get; private set; }
    
    public async Task<IDataTransaction> BeginTransactionAsync(IsolationLevel? isolationLevel = null)
    {
        if (CurrentTransaction != null) throw new TransactionAlreadyExistException();

        CurrentSession = await MongoClient.StartSessionAsync();

        CurrentTransaction = new MongoTransaction(this, CurrentSession, CompleteTransaction);

        return CurrentTransaction;
    }

    public async Task<IDataTransaction?> BeginNestedTransactionAsync(IsolationLevel? isolationLevel = null)
    {
        return CurrentTransaction != null 
            ? null 
            : await BeginTransactionAsync(isolationLevel);
    }

    public Task<IEnumerable<TResult>> ExecuteTableFunctionAsync<TEntity, TResult>(DataFunction<TEntity, TResult> function) where TEntity : class
    {
        throw new NotSupportedException("MongoDB not supported Table functions");
    }

    public Task<TResult?> ExecuteFunctionAsync<TEntity, TResult>(DataFunction<TEntity, TResult> function) where TEntity : class
    {
        throw new NotSupportedException("MongoDB not supported functions");
    }

    public IMongoCollection<TEntity> GetCollection<TEntity>(string name) where TEntity : class
    {
        return Store.GetCollection<TEntity>(name);
    }

    public IMongoDatabase GetStore()
    {
        return Store;
    }

    public void Dispose()
    {
        if (_disposed) return;

        CurrentTransaction?.Dispose();
        _disposed = true;
    }
    
    protected DatabaseOptions Options { get; }
    protected string DatabaseName { get; set; }
    protected MongoClient MongoClient { get; }
    protected IMongoDatabase Store { get; }
    protected IClientSessionHandle? CurrentSession { get; private set; }
    
    private bool _disposed;
    
    private void CompleteTransaction()
    {
        CurrentSession = null;
        CurrentTransaction = null;
    }
}