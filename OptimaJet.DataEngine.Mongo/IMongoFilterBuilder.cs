using MongoDB.Driver;
using OptimaJet.DataEngine.Filters;

namespace OptimaJet.DataEngine.Mongo;

public interface IMongoFilterBuilder<TEntity> where TEntity : class
{
    FilterDefinition<TEntity> Build(IFilter filter);
}