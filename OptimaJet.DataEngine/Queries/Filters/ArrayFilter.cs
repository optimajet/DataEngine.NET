namespace OptimaJet.DataEngine.Queries.Filters;

public sealed class ArrayFilter : ConstantFilter
{
    public ArrayFilter(object?[] values) : base(values)
    {
        Values = values;
    }

    public object?[] Values { get; }
    public override FilterType FilterType => FilterType.Array;
    
    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}