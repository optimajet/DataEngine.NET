namespace OptimaJet.DataEngine.Queries.Filters;

public sealed class IsNullFilter : PropertyUnaryFilter
{
    public IsNullFilter(PropertyFilter property) : base(property) {}

    public override FilterType FilterType => FilterType.IsNull;

    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}