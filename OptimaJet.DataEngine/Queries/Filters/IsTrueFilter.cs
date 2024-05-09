namespace OptimaJet.DataEngine.Queries.Filters;

public sealed class IsTrueFilter : PropertyUnaryFilter
{
    public IsTrueFilter(PropertyFilter property) : base(property) {}

    public override FilterType FilterType => FilterType.IsTrue;
    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}