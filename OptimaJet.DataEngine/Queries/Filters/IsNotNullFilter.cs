namespace OptimaJet.DataEngine.Queries.Filters;

public sealed class IsNotNullFilter : PropertyUnaryFilter
{
    public IsNotNullFilter(PropertyFilter property) : base(property) {}

    public override FilterType FilterType => FilterType.IsNotNull;

    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}