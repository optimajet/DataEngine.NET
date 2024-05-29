namespace OptimaJet.DataEngine.Queries.Filters;

public sealed class EqualFilter : PropertyConstantFilter
{
    public EqualFilter(PropertyFilter property, ConstantFilter constant) : base(property, constant) {}
    
    public override FilterType FilterType => FilterType.Equal;


    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}