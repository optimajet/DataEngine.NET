namespace OptimaJet.DataEngine.Queries.Filters;

public sealed class TrueFilter : ConstantFilter
{
    private static readonly Lazy<TrueFilter> LazyInstance = new(() => new TrueFilter());
    public static TrueFilter Instance => LazyInstance.Value;

    private TrueFilter() : base(true)
    {
    }

    public override FilterType FilterType => FilterType.True;

    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}