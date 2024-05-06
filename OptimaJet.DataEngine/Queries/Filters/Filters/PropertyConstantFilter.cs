namespace OptimaJet.DataEngine.Filters;

public abstract class PropertyConstantFilter : BinaryFilter
{
    protected PropertyConstantFilter(PropertyFilter property, ConstantFilter constant) : base(property, constant)
    {
        Property = property;
        Constant = constant;
    }

    public PropertyFilter Property { get; }
    public ConstantFilter Constant { get; }
}