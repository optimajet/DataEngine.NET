using System.Linq.Expressions;
using System.Reflection;
using OptimaJet.DataEngine.Exceptions;

namespace OptimaJet.DataEngine.Queries.Selector;

internal class FieldSelector : ExpressionVisitor, IFieldSelector
{
    internal FieldSelector() { }

    public string GetFieldName<TEntity, TField>(Expression<Func<TEntity, TField>> fFieldSelector)
    {
        _propertyName = null;
        
        Visit(fFieldSelector);

        if (_propertyName == null) throw new ExpressionTreeParsingException("Parameter not found");

        return _propertyName;
    }
    
    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Member is not PropertyInfo property) return node;

        if (_propertyName != null) throw new ExpressionTreeParsingException("Dual parameter definition");
        
        _propertyName = property.Name;
        
        return node;
    }

    private string? _propertyName;
}