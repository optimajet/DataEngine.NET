namespace OptimaJet.DataEngine.Queries.Filters;

public sealed class InFilter : BinaryFilter
{
    public InFilter(PropertyFilter property, ArrayFilter array) : base(property, array)
    {
        Property = property;
        Array = array;
    }

    public PropertyFilter Property { get; }
    public ArrayFilter Array { get; }
    public override FilterType FilterType => FilterType.In;


    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}