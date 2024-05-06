namespace OptimaJet.DataEngine.Filters;

public class ConstantFilter : Filter
{
    public ConstantFilter(object? value)
    {
        Value = value;
    }

    public object? Value { get; }
    public override FilterType FilterType => FilterType.Constant;

    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}