using System.Linq.Expressions;
using OptimaJet.DataEngine.Metadata;
using OptimaJet.DataEngine.Sql.Queries;

namespace OptimaJet.DataEngine.Sql.Implementation;

/// <summary>
/// A base class that implements the common logic of all relative database data sets.
/// </summary>
internal class SqlCollection<TEntity> : ICollection<TEntity> where TEntity : class
{
    public SqlCollection(SqlProvider provider)
    {
        Provider = provider;
    }

    public SqlProvider Provider { get; }
    public SqlSession Session => (SqlSession) Provider.Session;

    IProvider ICollection<TEntity>.Provider => Provider;
    ISession ICollection<TEntity>.Session => Session;

    public EntityMetadata Metadata => MetadataPool<TEntity>.GetMetadata(Provider.Name);

    #region Select

    public SelectConstructor<TEntity> Select() => new(this);

    public async Task<List<TEntity>> GetAsync(SelectConstructor<TEntity> constructor)
    {
        return await (await QueryFromAsync()).BuildByConstructor(constructor, Metadata).GetAsync<TEntity>(Metadata);
    }

    public async Task<TEntity?> FirstAsync(SelectConstructor<TEntity> constructor)
    {
        return await (await QueryFromAsync()).BuildByConstructor(constructor, Metadata).FirstAsync<TEntity>(Metadata);
    }

    public async Task<int> CountAsync(SelectConstructor<TEntity> constructor)
    {
        return await (await QueryFromAsync()).BuildByConstructor(constructor, Metadata).CountAsync();
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

    public async Task<TEntity?> GetByKeyAsync(object key)
    {
        return await (await QueryFromAsync()).Where(Metadata.PrimaryKeyColumn.Name, key).FirstAsync<TEntity>(Metadata);
    }

    public Task<List<TEntity>> GetByKeysAsync(IEnumerable<object> keys)
    {
        return GetByKeysAsync(keys.ToArray());
    }

    public async Task<List<TEntity>> GetByKeysAsync(params object[] keys)
    {
        return await (await QueryFromAsync()).WhereIn(Metadata.PrimaryKeyColumn.Name, keys).GetAsync<TEntity>(Metadata);
    }

    public async Task<List<TEntity>> GetAsync()
    {
        return await (await QueryFromAsync()).GetAsync<TEntity>(Metadata);
    }

    public async Task<TEntity?> FirstAsync()
    {
        return await (await QueryFromAsync()).FirstAsync<TEntity>(Metadata);
    }

    public async Task<int> CountAsync()
    {
        return await (await QueryFromAsync()).CountAsync();
    }

    #endregion

    #region Insert

    public async Task<int> InsertAsync(TEntity entity)
    {
        return await (await QueryFromAsync()).InsertAsync(Metadata.ToDictionary(entity));
    }

    public async Task<int> InsertAsync(IEnumerable<TEntity> entities)
    {
        return await (await QueryFromAsync()).InsertAsync(Metadata.ColumnNames,
            entities.Select(Metadata.GetColumnValues));
    }

    #endregion

    #region Update

    public UpdateConstructor<TEntity> Update() => new(this);

    public async Task<int> UpdateAsync(UpdateConstructor<TEntity> constructor)
    {
        if (constructor.Setter == null) return 0;
        return await (await QueryFromAsync()).Where(constructor.Filter, Metadata)
            .UpdateAsync(constructor.Setter.GetNamedFields(Metadata));
    }

    public async Task<int> UpdateAsync(TEntity entity, Expression<Predicate<TEntity>> fFilter)
    {
        return await (await QueryFromAsync()).Where(Update().Where(fFilter).Filter, Metadata)
            .UpdateAsync(Metadata.ToDictionary(entity));
    }

    public async Task<int> UpdateAsync(TEntity entity)
    {
        return await WherePk(await QueryFromAsync(), entity).UpdateAsync(Metadata.ToDictionary(entity));
    }

    #endregion

    #region Upsert

    public async Task<int> UpsertAsync(TEntity entity)
    {
        return await (await QueryFromAsync()).UpsertAsync(entity, Metadata);
    }

    #endregion

    #region Delete

    public DeleteConstructor<TEntity> Delete() => new(this);

    public async Task<int> DeleteAsync(DeleteConstructor<TEntity> constructor)
    {
        return await (await QueryFromAsync()).Where(constructor.Filter, Metadata).DeleteAsync();
    }

    public Task<int> DeleteAsync(Expression<Predicate<TEntity>> fFilter)
    {
        return Delete().Where(fFilter).ExecuteAsync();
    }

    public async Task<int> DeleteAsync(TEntity entity)
    {
        return await WherePk(await QueryFromAsync(), entity).DeleteAsync();
    }

    public async Task<int> DeleteAsync(IEnumerable<TEntity> entities)
    {
        return await WhereInPks(await QueryFromAsync(), entities).DeleteAsync();
    }

    public async Task<int> DeleteByKeyAsync(object key)
    {
        return await (await QueryFromAsync()).Where(Metadata.PrimaryKeyColumn.Name, key).DeleteAsync();
    }

    public Task<int> DeleteByKeysAsync(IEnumerable<object> keys)
    {
        return DeleteByKeysAsync(keys.ToArray());
    }

    public async Task<int> DeleteByKeysAsync(params object[] keys)
    {
        return await (await QueryFromAsync()).WhereIn(Metadata.PrimaryKeyColumn.Name, keys).DeleteAsync();
    }

    public async Task<int> DeleteAllAsync()
    {
        return await (await QueryFromAsync()).DeleteAsync();
    }

    #endregion

    #region Helpers

    private string TableName => String.IsNullOrWhiteSpace(Metadata.SchemaName)
        ? Metadata.Name
        : Metadata.SchemaName + "." + Metadata.Name;

    private async Task<DataQuery> QueryFromAsync()
    {
        return (await QueryAsync()).From(TableName);
    }

    private Task<DataQuery> QueryAsync()
    {
        return Session.CreateQueryAsync();
    }

    private DataQuery WherePk(DataQuery query, TEntity entity) =>
        query.Where(Metadata.PrimaryKeyColumn.Name, Metadata.PrimaryKeyColumn.GetValue(entity));

    private DataQuery WhereInPks(DataQuery query, IEnumerable<TEntity> entities) =>
        query.WhereIn(Metadata.PrimaryKeyColumn.Name, entities.Select(Metadata.PrimaryKeyColumn.GetValue));

    #endregion
}