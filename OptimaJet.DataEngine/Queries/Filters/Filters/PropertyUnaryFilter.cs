namespace OptimaJet.DataEngine.Filters;

public abstract class PropertyUnaryFilter : UnaryFilter
{
    protected PropertyUnaryFilter(PropertyFilter property) : base(property)
    {
        Property = property;
    }

    public PropertyFilter Property { get; }
}