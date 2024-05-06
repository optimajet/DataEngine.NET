namespace OptimaJet.DataEngine.Filters;

public sealed class GreaterEqualFilter : PropertyConstantFilter
{
    public GreaterEqualFilter(PropertyFilter property, ConstantFilter constant) : base(property, constant) {}
    
    public override FilterType FilterType => FilterType.GreaterEqual;


    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}