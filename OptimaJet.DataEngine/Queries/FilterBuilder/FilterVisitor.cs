using OptimaJet.DataEngine.Queries.Filters;

namespace OptimaJet.DataEngine.Queries.FilterBuilder;

internal class FilterVisitor : IFilterVisitor
{
    public virtual IFilter Visit(IFilter filter) => filter.Accept(this);

    public virtual IFilter Visit(PropertyFilter filter)
    {
        return filter;
    }

    public virtual IFilter Visit(ConstantFilter filter)
    {
        return filter;
    }

    public virtual IFilter Visit(TrueFilter filter)
    {
        return filter;
    }

    public virtual IFilter Visit(FalseFilter filter)
    {
        return filter;
    }

    public virtual IFilter Visit(NullFilter filter)
    {
        return filter;
    }

    public virtual IFilter Visit(AndFilter filter)
    {
        return VisitBinary(filter, (left, right) => new AndFilter(left, right));
    }

    public virtual IFilter Visit(OrFilter filter)
    {
        return VisitBinary(filter, (left, right) => new OrFilter(left, right));
    }

    public virtual IFilter Visit(NotFilter filter)
    {
        return VisitUnary(filter, operand => new NotFilter(operand));
    }

    public virtual IFilter Visit(EqualFilter filter)
    {
        return VisitBinary(filter, (left, right) => new EqualFilter((PropertyFilter) left, (ConstantFilter) right));
    }

    public virtual IFilter Visit(NotEqualFilter filter)
    {
        return VisitBinary(filter, (left, right) => new NotEqualFilter((PropertyFilter) left, (ConstantFilter) right));
    }

    public virtual IFilter Visit(GreaterFilter filter)
    {
        return VisitBinary(filter, (left, right) => new GreaterFilter((PropertyFilter) left, (ConstantFilter) right));
    }

    public virtual IFilter Visit(GreaterEqualFilter filter)
    {
        return VisitBinary(filter, (left, right) => new GreaterEqualFilter((PropertyFilter) left, (ConstantFilter) right));
    }

    public virtual IFilter Visit(LessFilter filter)
    {
        return VisitBinary(filter, (left, right) => new LessFilter((PropertyFilter) left, (ConstantFilter) right));
    }

    public virtual IFilter Visit(LessEqualFilter filter)
    {
        return VisitBinary(filter, (left, right) => new LessEqualFilter((PropertyFilter) left, (ConstantFilter) right));
    }

    public virtual IFilter Visit(IsNullFilter filter)
    {
        return VisitUnary(filter, operand => new IsNullFilter((PropertyFilter) operand));
    }

    public virtual IFilter Visit(IsNotNullFilter filter)
    {
        return VisitUnary(filter, operand => new IsNotNullFilter((PropertyFilter) operand));
    }

    public virtual IFilter Visit(IsTrueFilter filter)
    {
        return VisitUnary(filter, operand => new IsTrueFilter((PropertyFilter) operand));
    }

    public virtual IFilter Visit(IsFalseFilter filter)
    {
        return VisitUnary(filter, operand => new IsFalseFilter((PropertyFilter) operand));
    }

    public virtual IFilter Visit(LikeFilter filter)
    {
        return VisitBinary(filter, (left, right) => new LikeFilter((PropertyFilter) left, (LikePatternFilter) right));
    }

    public virtual IFilter Visit(LikePatternFilter filter)
    {
        return VisitUnary(filter, operand => new LikePatternFilter((ConstantFilter) operand, filter.Type));
    }

    public virtual IFilter Visit(InFilter filter)
    {
        return VisitBinary(filter, (left, right) => new InFilter((PropertyFilter) left, (ArrayFilter) right));
    }

    public virtual IFilter Visit(ArrayFilter filter)
    {
        return filter;
    }

    private IFilter VisitBinary(BinaryFilter filter, Func<IFilter, IFilter, IFilter> fCreateFilter)
    {
        var left = Visit(filter.Left);
        var right = Visit(filter.Right);

        if (filter.Left != left || filter.Right != right)
        {
            return fCreateFilter(left, right);
        }

        return filter;
    }
    
    private IFilter VisitUnary(UnaryFilter filter, Func<IFilter, IFilter> fCreateFilter)
    {
        var operand = Visit(filter.Operand);

        if (filter.Operand != operand)
        {
            return fCreateFilter(operand);
        }

        return filter;
    }
}