using OptimaJet.DataEngine.Exceptions;

namespace OptimaJet.DataEngine.Queries.Filters;

public sealed class LikePatternFilter : UnaryFilter
{
    public LikePatternFilter(ConstantFilter constant, LikePatternType type) : base(constant)
    {
        Constant = constant;
        Type = type;
    }

    public string Value => Type switch
    {
        LikePatternType.StartsWith => $"{Constant.Value}%",
        LikePatternType.EndsWith => $"%{Constant.Value}",
        LikePatternType.ContainsIn => $"%{Constant.Value}%",
        _ => throw new PatternTypeNotSupportedException()
    };
    
    public ConstantFilter Constant { get; }
    public LikePatternType Type { get; }
    public override FilterType FilterType => FilterType.LikePattern;

    public override IFilter Accept(IFilterVisitor visitor)
    {
        return visitor.Visit(this);
    }
}