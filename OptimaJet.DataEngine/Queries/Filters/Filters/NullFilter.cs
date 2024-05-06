namespace OptimaJet.DataEngine.Filters;

public sealed class NullFilter : ConstantFilter
{
    private static readonly Lazy<NullFilter> LazyInstance = new(() => new NullFilter());
    public static NullFilter Instance => LazyInstance.Value;

    private NullFilter() : base(null)
    {
    }

    public override FilterType FilterType => FilterType.Null;

    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}