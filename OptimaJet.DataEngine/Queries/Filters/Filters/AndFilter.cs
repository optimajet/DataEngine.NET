namespace OptimaJet.DataEngine.Filters;

public sealed class AndFilter : BinaryFilter
{
    public AndFilter(IFilter left, IFilter right) : base(left, right) {}
    
    public override FilterType FilterType => FilterType.And;
    
    public override IFilter Accept(IFilterVisitor visitor)
    {
         return visitor.Visit(this);
    }
}