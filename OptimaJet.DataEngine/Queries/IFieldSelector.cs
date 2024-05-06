using System.Linq.Expressions;

namespace OptimaJet.DataEngine;

public interface IFieldSelector
{
    public string GetFieldName<TEntity, TField>(Expression<Func<TEntity, TField>> fFieldSelector);
}