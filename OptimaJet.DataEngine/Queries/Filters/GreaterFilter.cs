namespace OptimaJet.DataEngine.Queries.Filters;

public sealed class GreaterFilter : PropertyConstantFilter
{
    public GreaterFilter(PropertyFilter property, ConstantFilter constant) : base(property, constant) {}
    
    public override FilterType FilterType => FilterType.Greater;


    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}