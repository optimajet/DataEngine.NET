namespace OptimaJet.DataEngine.Filters;

public sealed class PropertyFilter : Filter
{
    public PropertyFilter(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public override FilterType FilterType => FilterType.Property;

    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}