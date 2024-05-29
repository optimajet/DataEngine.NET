namespace OptimaJet.DataEngine.Queries.Filters;

public sealed class LessFilter : PropertyConstantFilter
{
    public LessFilter(PropertyFilter property, ConstantFilter constant) : base(property, constant) {}
    
    public override FilterType FilterType => FilterType.Less;


    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}