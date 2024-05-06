namespace OptimaJet.DataEngine.Filters;

public sealed class NotEqualFilter : PropertyConstantFilter
{
    public NotEqualFilter(PropertyFilter property, ConstantFilter constant) : base(property, constant) {}
    
    public override FilterType FilterType => FilterType.NotEqual;


    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}