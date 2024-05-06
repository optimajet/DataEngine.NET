using System.Linq.Expressions;

namespace OptimaJet.DataEngine.Filters;

public interface IFilterBuilder
{
    public IFilter Build<TEntity>(Expression<Predicate<TEntity>> fFilter);
}