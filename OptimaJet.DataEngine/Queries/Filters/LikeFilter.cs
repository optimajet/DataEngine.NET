namespace OptimaJet.DataEngine.Queries.Filters;

public sealed class LikeFilter : BinaryFilter
{
    public LikeFilter(PropertyFilter property, LikePatternFilter likePattern) : base(property, likePattern)
    {
        Property = property;
        LikePattern = likePattern;
    }

    public PropertyFilter Property { get; }
    public LikePatternFilter LikePattern { get; }
    public override FilterType FilterType => FilterType.Like;


    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}