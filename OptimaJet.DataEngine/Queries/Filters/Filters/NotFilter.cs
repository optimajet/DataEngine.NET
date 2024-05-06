namespace OptimaJet.DataEngine.Filters;

public sealed class NotFilter : UnaryFilter
{
    public NotFilter(IFilter operand) : base(operand) {}

    public override FilterType FilterType => FilterType.Not;

    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}