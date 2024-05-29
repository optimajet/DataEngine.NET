using System.Linq.Expressions;
using OptimaJet.DataEngine.Queries.Filters;

namespace OptimaJet.DataEngine.Queries.FilterBuilder;

internal class FilterBuilder : ExpressionVisitor, IFilterBuilder
{
    public FilterBuilder()
    {
        Node = new Node();
    }

    public IFilter Build<TEntity>(Expression<Predicate<TEntity>> fFilter)
    {
        Node = new Node();
        
        var modified = Visit(fFilter) ?? throw new InvalidOperationException();

        Node.SetBuildType();
        
        Node.Content.SetExpression(modified);
        
        Node.ReduceIfPossible();

        return Node.Content.Filter ?? FalseFilter.Instance;
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        var buildable = new[]
        {
            ExpressionType.AndAlso,
            ExpressionType.OrElse,
            ExpressionType.Equal,
            ExpressionType.NotEqual,
            ExpressionType.GreaterThan,
            ExpressionType.GreaterThanOrEqual,
            ExpressionType.LessThan,
            ExpressionType.LessThanOrEqual,
        };
        
        var executable = new[]
        {
            ExpressionType.Add,
            ExpressionType.AddChecked,
            ExpressionType.Divide,
            ExpressionType.Modulo,
            ExpressionType.Multiply,
            ExpressionType.MultiplyChecked,
            ExpressionType.Power,
            ExpressionType.Subtract,
            ExpressionType.SubtractChecked,
            ExpressionType.And,
            ExpressionType.Or,
            ExpressionType.ExclusiveOr,
            ExpressionType.LeftShift,
            ExpressionType.RightShift,
            ExpressionType.Coalesce,
            ExpressionType.ArrayIndex,
        };

        Node = new Node(Node, buildable.Contains(node.NodeType), executable.Contains(node.NodeType));

        var modified = base.VisitBinary(node);

        Node.Content.SetExpression(modified);
        
        Node.ReduceIfPossible();

        Node = Node.Parent!;
        
        return modified;
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
        var ignore = new[]
        {
            ExpressionType.Convert,
        };

        if (ignore.Contains(node.NodeType)) return base.VisitUnary(node);
        
        var buildable = new[]
        {
            ExpressionType.Not,
        };
        
        var executable = new[]
        {
            ExpressionType.ArrayLength,
            ExpressionType.ConvertChecked,
            ExpressionType.Negate,
            ExpressionType.NegateChecked,
            ExpressionType.TypeAs,
            ExpressionType.UnaryPlus,
        };

        Node = new Node(Node, buildable.Contains(node.NodeType), executable.Contains(node.NodeType));
        
        var modified = base.VisitUnary(node);

        Node.Content.SetExpression(modified);
        
        Node.ReduceIfPossible();

        Node = Node.Parent!;
        
        return modified;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        var buildable = new[]
        {
            "System.String.Contains",
            "System.Linq.Enumerable.Contains",
            "System.Collections.Generic.List`1[System.String].Contains",
        };

        var fullName = node.Method.GetFullName();

        Node = new Node(Node, buildable.Contains(fullName));
        
        var modified = base.VisitMethodCall(node);
        
        Node.Content.SetExpression(modified);
        
        Node.ReduceIfPossible();

        Node = Node.Parent!;

        return modified;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        Node = new Node(Node, endNode: true);
        
        Node.Content.SetExpression(node);

        Node = Node.Parent!;

        return node;
    }

    protected override Expression VisitConstant(ConstantExpression expression)
    {
        if (Node.Type is NodeType.Build or NodeType.Specify)
        {
            Node = new Node(Node, endNode: true);
            
            Node.Content.SetExpression(expression);
            
            Node = Node.Parent!;
        }

        //Ignored
        
        return expression;
    }
    
    /// <summary>
    /// Current Node
    /// </summary>
    private Node Node { get; set; }
}