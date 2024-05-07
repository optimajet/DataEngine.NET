using System.Linq.Expressions;
using MongoDB.Driver;
using OptimaJet.DataEngine.Filters;
using OptimaJet.DataEngine.Metadata;
using OptimaJet.DataEngine.Setters;
using OptimaJet.DataEngine.Sorts;

namespace OptimaJet.DataEngine.Mongo;

public class MongoCollection<TEntity> : ICollection<TEntity> where TEntity : class
{
    public MongoCollection(MongoProvider provider)
    {
        Provider = provider;
    }

    public MongoProvider Provider { get; }
    public MongoSession Session => (MongoSession) Provider.Session;

    IProvider ICollection<TEntity>.Provider => Provider;
    ISession ICollection<TEntity>.Session => Session;

    public EntityMetadata Metadata => MetadataPool<TEntity>.GetMetadata(Provider.Name);

    #region Select

    public SelectConstructor<TEntity> Select() => new(this);

    public async Task<List<TEntity>> GetAsync(SelectConstructor<TEntity> constructor)
    {
        var mongoFilter = GetMongoFilter(constructor.Filter);
        var mongoSort = GetMongoSort(constructor.Sort);

        return await GetCollection()
            .Find(mongoFilter)
            .Sort(mongoSort)
            .Skip(constructor.Offset)
            .Limit(constructor.Limit)
            .ToListAsync();
    }

    public async Task<TEntity?> FirstAsync(SelectConstructor<TEntity> constructor)
    {
        var mongoFilter = GetMongoFilter(constructor.Filter);
        var mongoSort = GetMongoSort(constructor.Sort);

        return await GetCollection()
            .Find(mongoFilter)
            .Sort(mongoSort)
            .Skip(constructor.Offset)
            .Limit(constructor.Limit)
            .FirstOrDefaultAsync();
    }

    public async Task<int> CountAsync(SelectConstructor<TEntity> constructor)
    {
        var mongoFilter = GetMongoFilter(constructor.Filter);
        var mongoSort = GetMongoSort(constructor.Sort);
        
        return Convert.ToInt32(
            await GetCollection()
                .Find(mongoFilter)
                .Sort(mongoSort)
                .Skip(constructor.Offset)
                .Limit(constructor.Limit)
                .CountDocumentsAsync()
        );
    }

    public Task<List<TEntity>> GetAsync(Expression<Predicate<TEntity>> fFilter)
    {
        return Select().Where(fFilter).GetAsync();
    }

    public Task<TEntity?> FirstAsync(Expression<Predicate<TEntity>> fFilter)
    {
        return Select().Where(fFilter).FirstAsync();
    }

    public Task<int> CountAsync(Expression<Predicate<TEntity>> fFilter)
    {
        return Select().Where(fFilter).CountAsync();
    }

    public Task<TEntity?> GetByKeyAsync(object key)
    {
        return Select().Where(GetPrimaryKeyFilter(key)).FirstAsync();
    }

    public Task<List<TEntity>> GetByKeysAsync(IEnumerable<object> keys)
    {
        return GetByKeysAsync(keys.ToArray());
    }

    public Task<List<TEntity>> GetByKeysAsync(params object[] keys)
    {
        return Select().Where(GetPrimaryKeyFilter(keys.ToArray())).GetAsync();
    }

    public Task<List<TEntity>> GetAsync()
    {
        return GetCollection().Find(EmptyFilter).ToListAsync();
    }

    public Task<TEntity?> FirstAsync()
    {
        return GetCollection().Find(EmptyFilter).FirstOrDefaultAsync()!;
    }

    public async Task<int> CountAsync()
    {
        return Convert.ToInt32(await GetCollection().Find(EmptyFilter).CountDocumentsAsync());
    }

    #endregion

    #region Insert

    public async Task<int> InsertAsync(TEntity entity)
    {
        await GetCollection().InsertOneAsync(entity);
        return 1;
    }

    public async Task<int> InsertAsync(IEnumerable<TEntity> entities)
    {
        var list = entities.ToList();

        if (list.Count == 0) return 0;
        
        await GetCollection().InsertManyAsync(list);
        return list.Count;
    }

    #endregion

    #region Update

    public UpdateConstructor<TEntity> Update() => new(this);

    public async Task<int> UpdateAsync(UpdateConstructor<TEntity> constructor)
    {
        var mongoFilter = GetMongoFilter(constructor.Filter);
        var mongoSetter = GetMongoSetter(constructor.Setter);

        var result = await GetCollection().UpdateManyAsync(mongoFilter, mongoSetter);

        return Convert.ToInt32(result.ModifiedCount);
    }

    public Task<int> UpdateAsync(TEntity entity, Expression<Predicate<TEntity>> fFilter)
    {
        return Update().Where(fFilter).Set(GetSetter(entity)).ExecuteAsync();
    }

    public Task<int> UpdateAsync(TEntity entity)
    {
        return Update().Where(GetEntityFilter(entity)).Set(GetSetter(entity)).ExecuteAsync();
    }

    #endregion

    #region Upsert

    public async Task<int> UpsertAsync(TEntity entity)
    {
        var mongoFilter = GetMongoFilter(GetEntityFilter(entity));
        var setter = GetMongoSetter(GetSetter(entity));
        
        var result = await GetCollection().UpdateOneAsync(mongoFilter, setter, new UpdateOptions {IsUpsert = true});
        
        return Convert.ToInt32(result.ModifiedCount);
    }

    #endregion

    #region Delete

    public DeleteConstructor<TEntity> Delete() => new(this);

    public async Task<int> DeleteAsync(DeleteConstructor<TEntity> constructor)
    {
        var mongoFilter = GetMongoFilter(constructor.Filter);
        var result = await GetCollection().DeleteManyAsync(mongoFilter);
        return Convert.ToInt32(result.DeletedCount);
    }

    public Task<int> DeleteAsync(Expression<Predicate<TEntity>> fFilter)
    {
        return Delete().Where(fFilter).ExecuteAsync();
    }

    public Task<int> DeleteAsync(TEntity entity)
    {
        return Delete().Where(GetEntityFilter(entity)).ExecuteAsync();
    }

    public Task<int> DeleteAsync(IEnumerable<TEntity> entities)
    {
        return Delete().Where(GetEntityFilter(entities.ToArray())).ExecuteAsync();
    }

    public Task<int> DeleteByKeyAsync(object key)
    {
        return Delete().Where(GetPrimaryKeyFilter(key)).ExecuteAsync();
    }

    public Task<int> DeleteByKeysAsync(IEnumerable<object> keys)
    {
        return DeleteByKeysAsync(keys.ToArray());
    }

    public Task<int> DeleteByKeysAsync(params object[] keys)
    {
        return Delete().Where(GetPrimaryKeyFilter(keys.ToArray())).ExecuteAsync();
    }

    public async Task<int> DeleteAllAsync()
    {
        var result = await GetCollection().DeleteManyAsync(EmptyFilter);
        return Convert.ToInt32(result.DeletedCount);
    }

    #endregion
    
    #region Helpers

    private FilterDefinition<TEntity> EmptyFilter => FilterDefinition<TEntity>.Empty;
    
    private IMongoCollection<TEntity> GetCollection() => Session.GetCollection<TEntity>(Metadata.Name);
    
    private IFilter GetPrimaryKeyFilter(params object[] keys)
    {
        if (keys.Length == 0) return FalseFilter.Instance;

        var propertyFilter = new PropertyFilter(Metadata.PrimaryKeyColumn.Name);
        return keys.Length > 1 
            ? new InFilter(propertyFilter, new ArrayFilter(keys.ToArray()))
            : new EqualFilter(propertyFilter, new ConstantFilter(keys.First()));
    }
    
    private IFilter GetEntityFilter(params TEntity[] entities)
    {
        if (entities.Length == 0) return FalseFilter.Instance;
        
        return entities.Length > 1 
            ? new InFilter(
                new PropertyFilter(Metadata.PrimaryKeyColumn.Name), 
                new ArrayFilter(entities.Select(e => Metadata.PrimaryKeyColumn.GetValue(e)).ToArray()))
            : new EqualFilter(
                new PropertyFilter(Metadata.PrimaryKeyColumn.Name), 
                new ConstantFilter(Metadata.PrimaryKeyColumn.GetValue(entities.First())));
    }

    private FilterDefinition<TEntity> GetMongoFilter(IFilter? filter)
    {
        if (filter == null) return EmptyFilter;

        return MongoFilterBuilder<TEntity>.Create(Metadata).Build(filter);
    }
    
    private SortDefinition<TEntity>? GetMongoSort(Sort? sort)
    {
        var builder = Builders<TEntity>.Sort;
        
        if (sort == null) return builder.Combine();

        var orders = sort.Orders;

        var mongoOrders = orders.Select(order => order.Direction switch
        {
            Direction.Asc => builder.Ascending(Metadata.GetColumnNameByProperty(order.OriginalName)),
            Direction.Desc => builder.Descending(Metadata.GetColumnNameByProperty(order.OriginalName)),
            _ => throw new ArgumentOutOfRangeException(nameof(order.Direction))
        });

        return builder.Combine(mongoOrders);
    }
    
    private UpdateDefinition<TEntity>? GetMongoSetter(Setter? setter)
    {
        if (setter == null) return null;
        
        var builder = Builders<TEntity>.Update;

        var mongoFields = setter.GetNamedFields(Metadata).Select(pair => builder.Set(pair.Key, pair.Value));

        return builder.Combine(mongoFields);
    }
    
    private Setter GetSetter(TEntity entity)
    {
        var setter = new Setter();

        foreach (var column in Metadata.NotPkColumns)
        {
            setter.Fields[column.OriginalName] = column.GetValue(entity);
        }

        return setter;
    }

    #endregion
}