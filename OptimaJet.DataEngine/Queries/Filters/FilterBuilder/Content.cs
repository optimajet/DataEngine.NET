using System.Collections;
using System.Linq.Expressions;
using OptimaJet.DataEngine.Exceptions;

namespace OptimaJet.DataEngine.Filters;

internal class Content
{
    public Content(Node node)
    {
        _node = node;
        Type = ContentType.Empty;
    }
    
    public ContentType Type { get; private set; }
    public Expression? Expression { get; private set; }
    public IFilter? Filter { get; private set; }

    public IFilter GetFilterOrToFilter()
    {
        return Type == ContentType.Filter 
            ? Filter! 
            : ToFilter();
    }

    public IFilter ToFilter()
    {
        IFilter? filter;
        
        switch (Type)
        {
            case ContentType.Filter:
                return Filter!;
            case ContentType.Constant:
            {
                var expression = (ConstantExpression) Expression!;
                var value = expression.Value;

                filter = value switch
                {
                    null => NullFilter.Instance,
                    bool b => b ? TrueFilter.Instance : FalseFilter.Instance,
                    string s => new ConstantFilter(s),
                    IEnumerable e => new ArrayFilter(e.Cast<object>().ToArray()),
                    _ => new ConstantFilter(value)
                };

                break;
            }
            case ContentType.Property:
            {
                var expression = (MemberExpression) Expression!;
                filter = new PropertyFilter(expression.Member.Name);
                break;
            }
            default:
                throw new ContentTypeOutOfRangeException();
        }

        Type = ContentType.Filter;
        Filter = filter ?? throw new FilterCreationException();
        Expression = null;

        return Filter;
    }
    
    public void SetFilter(IFilter filter)
    {
        Type = ContentType.Filter;
        Filter = filter;
        Expression = null;
    }

    public void SetExpression(Expression expression)
    {
        Type = ContentType.Expression;
        Expression = expression;
        Filter = null;
    }

    public void SetExpression(MemberExpression expression)
    {
        var nested = expression.Expression;

        
        switch (nested?.NodeType)
        {
            case ExpressionType.Parameter:
                if (_node.Parent?.Type is NodeType.Execute or NodeType.ExecutableCheck or null)
                {
                    throw new ExpressionTreeParsingException("The presence of a parameter in an executable branch" +
                                                             " of the expression tree is not allowed.");
                }

                _node.Parent.SetBuildType();
                Type = ContentType.Property;
                Expression = expression;
                Filter = null;
                break;
            case ExpressionType.Constant:
                Type = ContentType.Constant;
                
                Expression = nested.Type.IsClass 
                    ? Expression.Constant(Execute(expression)) 
                    : nested;
            
                Filter = null;
                break;
            default:
                Type = ContentType.Constant;
            
                Expression = Expression.Constant(Execute(expression));
            
                Filter = null;
                break;
        }
    }

    public void SetExpression(ConstantExpression expression)
    {
        Type = ContentType.Constant;
        Expression = expression;
        Filter = null;
    }

    public void ToConstant()
    {
        if (Expression is null) throw new NullExpressionException();

        Type = ContentType.Constant;
        Expression = Expression.Constant(Execute(Expression));
        Filter = null;
    }

    public void Clear()
    {
        Type = ContentType.Empty;
        Expression = null;
        Filter = null;
    }

    private static object? Execute(Expression expression)
    {
        var lambda = Expression.Lambda(expression);
        var func = lambda.Compile();

        return func.DynamicInvoke();
    }

    private readonly Node _node;
}