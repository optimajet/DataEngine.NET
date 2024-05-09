using System.Linq.Expressions;
using OptimaJet.DataEngine.Exceptions;
using OptimaJet.DataEngine.Queries.Filters;

namespace OptimaJet.DataEngine.Queries.FilterBuilder;

internal class Node
{
    public Node(Node? parent = null, bool buildable = true, bool executable = true, bool endNode = false)
    {
        Parent = parent;
        Parent?.Children.Add(this);
        Type = endNode ? NodeType.EndNode : GetChildType(Parent?.Type, buildable, executable);
        Children = new List<Node>();
        Content = new Content(this);
    }

    public NodeType Type { get; private set; }
    public Node? Parent { get; }
    public List<Node> Children { get; }
    public Content Content { get; }

    public void SetBuildType()
    {
        if (Type is not (NodeType.Specify or NodeType.Build))
        {
            throw new ExpressionTreeParsingException("You cannot assign a build type to a node " +
                                                     "if its ancestors are of a type other than Specify or Build.");
        }

        Type = NodeType.Build;

        Parent?.SetBuildType();
    }

    public void ReduceIfPossible()
    {
        switch (Type)
        {
            case NodeType.EndNode:
                break;
            case NodeType.ExecutableCheck:
                Children.Clear();
                Content.Clear();
                break;
            case NodeType.Execute or NodeType.Specify:
                Content.ToConstant();
                Children.Clear();
                Type = NodeType.EndNode;
                break;
            case NodeType.Build:
                SetFilterContent(ToFilter());
                break;
        }
    }

    private static NodeType GetChildType(NodeType? parentType, bool buildable, bool executable)
    {
        if (parentType is NodeType.Execute or NodeType.ExecutableCheck) return NodeType.ExecutableCheck;
        
        if (buildable || parentType is null) return NodeType.Specify;
        if (executable) return NodeType.Execute;

        throw new NodeCreationException();
    }

    #region ToFilter

    private IFilter ToFilter()
    {
        return Content.Expression?.NodeType switch
        {
            ExpressionType.AndAlso 
                or ExpressionType.OrElse => ToBinaryFilter(),
            ExpressionType.Equal 
                or ExpressionType.NotEqual 
                or ExpressionType.GreaterThan 
                or ExpressionType.GreaterThanOrEqual 
                or ExpressionType.LessThan 
                or ExpressionType.LessThanOrEqual => ToPropertyConstantFilter(),
            ExpressionType.Not => ToNotFilter(),
            ExpressionType.Call => ToMethodBasedFilter(),
            ExpressionType.Lambda => ToBodyFilter(),
            _ => throw new FilterCreationException($"Cannot create filter for expression type {Content.Expression?.NodeType.ToString()}")
        };
    }

    private IFilter ToBodyFilter()
    {
        var body = Children.FirstOrDefault()?.Content ?? throw new FilterCreationException("Missing body to create body filter");

        var type = body.Type;

        return type switch
        {
            ContentType.Property => new IsTrueFilter((PropertyFilter) body.GetFilterOrToFilter()),
            ContentType.Constant => body.GetFilterOrToFilter() is TrueFilter ? TrueFilter.Instance : FalseFilter.Instance,
            ContentType.Filter => body.Filter!,
            _ => FalseFilter.Instance,
        };
    }

    private IFilter ToBinaryFilter()
    {
        if (Children.Count != 2) throw new FilterCreationException("Invalid number of children to create a binary filter");
        
        var leftChild = Children[0] ?? throw new FilterCreationException("The first child is null");
        var left = leftChild.Content.ToFilter();
        
        var rightChild = Children[1] ?? throw new FilterCreationException("The second child is null");
        var right = rightChild.Content.ToFilter();

        var operands = new[] { left, right };
        var operandsTypes = new[] { left.FilterType, right.FilterType };
        
        var type = Content.Expression?.NodeType;

        return type switch
        {
            ExpressionType.AndAlso =>
                operandsTypes.Contains(FilterType.False)
                    ? FalseFilter.Instance
                    : operandsTypes.All(t => t is FilterType.True) 
                        ? TrueFilter.Instance
                        : operandsTypes.Contains(FilterType.True)
                            ? operands.First(o => o.FilterType != FilterType.True)
                            : new AndFilter(PrepareBinaryOperand(left), PrepareBinaryOperand(right)),
            ExpressionType.OrElse =>
                operandsTypes.Contains(FilterType.True)
                    ? TrueFilter.Instance
                    : operandsTypes.All(t => t is FilterType.False) 
                        ? FalseFilter.Instance
                        : operandsTypes.Contains(FilterType.False)
                            ? operands.First(o => o.FilterType != FilterType.False)
                            : new OrFilter(PrepareBinaryOperand(left), PrepareBinaryOperand(right)),
            _ => throw new FilterCreationException("Wrong type of expression to create binary filter")
        };
    }

    private IFilter PrepareBinaryOperand(IFilter operand)
    {
        return operand.FilterType == FilterType.Property 
            ? new IsTrueFilter((PropertyFilter) operand) 
            : operand;
    }

    private IFilter ToPropertyConstantFilter()
    {
        if (Children.Count != 2) throw new FilterCreationException("Invalid number of children to create a property-constant filter");
        
        var firstChild = Children.FirstOrDefault(c => c.Content.Type == ContentType.Property) 
                         ?? throw new FilterCreationException("The first child is null");
        
        var property = (PropertyFilter) firstChild.Content.GetFilterOrToFilter();
        
        var secondChild = Children.FirstOrDefault(c => c.Content.Type == ContentType.Constant) 
                          ?? throw new FilterCreationException("The second child is null");
        
        var constant = (ConstantFilter) secondChild.Content.GetFilterOrToFilter();

        var type = Content.Expression?.NodeType;

        return type switch
        {
            ExpressionType.Equal => constant.FilterType switch
            {
                FilterType.Null => new IsNullFilter(property),
                _ => new EqualFilter(property, constant)
            },
            ExpressionType.NotEqual => constant.FilterType switch
            {
                FilterType.Null => new IsNotNullFilter(property),
                _ => new NotEqualFilter(property, constant)
            },
            ExpressionType.GreaterThan => new GreaterFilter(property, constant),
            ExpressionType.GreaterThanOrEqual => new GreaterEqualFilter(property, constant),
            ExpressionType.LessThan => new LessFilter(property, constant),
            ExpressionType.LessThanOrEqual => new LessEqualFilter(property, constant),
            _ => throw new FilterCreationException("Wrong type of expression to create property-constant filter")
        };
    }
    
    private IFilter ToNotFilter()
    {
        if (Children.Count != 1) throw new FilterCreationException("Invalid number of children to create a not filter");
        
        var child = Children.First() ?? throw new FilterCreationException("The first child is null");
        
        var type = child.Content.Type;

        return type switch
        {
            ContentType.Property => new IsFalseFilter((PropertyFilter) child.Content.GetFilterOrToFilter()),
            ContentType.Constant => child.Content.GetFilterOrToFilter().FilterType switch
            {
                FilterType.True => FalseFilter.Instance,
                FilterType.False => TrueFilter.Instance,
                _ => throw new FilterCreationException("Unable to create a not filter for non-boolean constants")
            },
            ContentType.Filter => PrepareUnaryOperand(child.Content.Filter!),
            _ => throw new FilterCreationException("Wrong content type to create not filter")
        };
    }
    
    private IFilter PrepareUnaryOperand(IFilter operand)
    {
        return operand.FilterType == FilterType.Property 
            ? new IsFalseFilter((PropertyFilter) operand) 
            : new NotFilter(operand);
    }
    
    private IFilter ToMethodBasedFilter()
    {
        var expression = Content.Expression as MethodCallExpression 
                         ?? throw new FilterCreationException("No expression to create method-based filter");

        var children = Children.Where(c => c.Content.Type != ContentType.Property).ToList();
        
        var propertyChild = Children.FirstOrDefault(c => c.Content.Type == ContentType.Property);
        var property = (PropertyFilter?) propertyChild?.Content.GetFilterOrToFilter();

        var name = expression.Method.GetFullName();

        return name switch
        {
            "System.String.Contains" => ToLikeFilter(property, children.First(), LikePatternType.ContainsIn),
            "System.Linq.Enumerable.Contains" => ToInFilter(property, children.First()),
            "System.Collections.Generic.List`1[System.String].Contains" =>  ToInFilter(property, children.First()),
            _ => throw new FilterCreationException("Wrong method name to create method-based filter")
        };
    }

    private IFilter ToLikeFilter(PropertyFilter? property, Node node, LikePatternType type)
    {
        return new LikeFilter(
            property ?? throw new FilterCreationException("The property is null"),
            new LikePatternFilter((ConstantFilter) node.Content.GetFilterOrToFilter(), type));
    }

    private IFilter ToInFilter(PropertyFilter? property, Node node)
    {
        return new InFilter(
            property ?? throw new FilterCreationException("The property is null"), 
            (ArrayFilter) node.Content.GetFilterOrToFilter());
    }

    private void SetFilterContent(IFilter filter)
    {
        Content.SetFilter(filter);
        Type = NodeType.EndNode;
        Children.Clear();
    }

    #endregion
}