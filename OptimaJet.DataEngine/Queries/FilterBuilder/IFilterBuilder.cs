using System.Linq.Expressions;

namespace OptimaJet.DataEngine.Queries.FilterBuilder;

internal interface IFilterBuilder
{
    public IFilter Build<TEntity>(Expression<Predicate<TEntity>> fFilter);
}