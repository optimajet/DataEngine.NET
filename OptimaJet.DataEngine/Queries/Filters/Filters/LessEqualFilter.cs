namespace OptimaJet.DataEngine.Filters;

public sealed class LessEqualFilter : PropertyConstantFilter
{
    public LessEqualFilter(PropertyFilter property, ConstantFilter constant) : base(property, constant) {}
    
    public override FilterType FilterType => FilterType.LessEqual;


    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}