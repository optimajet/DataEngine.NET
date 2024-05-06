namespace OptimaJet.DataEngine.Filters;

public sealed class OrFilter : BinaryFilter
{
    public OrFilter(IFilter left, IFilter right) : base(left, right) {}
    
    public override FilterType FilterType => FilterType.Or;
    
    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}