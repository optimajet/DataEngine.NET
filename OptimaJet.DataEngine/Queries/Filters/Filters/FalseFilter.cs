namespace OptimaJet.DataEngine.Filters;

public sealed class FalseFilter : ConstantFilter
{
    private static readonly Lazy<FalseFilter> LazyInstance = new(() => new FalseFilter());
    public static FalseFilter Instance => LazyInstance.Value;

    private FalseFilter() : base(false)
    {
    }

    public override FilterType FilterType => FilterType.False;

    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}