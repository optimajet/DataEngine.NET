namespace OptimaJet.DataEngine.Queries.Filters;

public sealed class IsFalseFilter : PropertyUnaryFilter
{
    public IsFalseFilter(PropertyFilter property) : base(property) {}

    public override FilterType FilterType => FilterType.IsFalse;
    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}