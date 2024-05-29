using MongoDB.Driver;
using OptimaJet.DataEngine.Queries;

namespace OptimaJet.DataEngine.Mongo.FilterBuilder;

internal interface IMongoFilterBuilder<TEntity> where TEntity : class
{
    FilterDefinition<TEntity> Build(IFilter filter);
}