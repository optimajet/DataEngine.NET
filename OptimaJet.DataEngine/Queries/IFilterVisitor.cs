using OptimaJet.DataEngine.Queries.Filters;

namespace OptimaJet.DataEngine.Queries;

public interface IFilterVisitor
{
    IFilter Visit(PropertyFilter filter);
    IFilter Visit(ConstantFilter filter);
    IFilter Visit(TrueFilter filter);
    IFilter Visit(FalseFilter filter);
    IFilter Visit(NullFilter filter);
    IFilter Visit(AndFilter filter);
    IFilter Visit(OrFilter filter);
    IFilter Visit(NotFilter filter);
    IFilter Visit(EqualFilter filter);
    IFilter Visit(NotEqualFilter filter);
    IFilter Visit(GreaterFilter filter);
    IFilter Visit(GreaterEqualFilter filter);
    IFilter Visit(LessFilter filter);
    IFilter Visit(LessEqualFilter filter);
    IFilter Visit(IsNullFilter filter);
    IFilter Visit(IsNotNullFilter filter);
    IFilter Visit(IsTrueFilter filter);
    IFilter Visit(IsFalseFilter filter);
    IFilter Visit(LikeFilter filter);
    IFilter Visit(LikePatternFilter filter);
    IFilter Visit(InFilter filter);
    IFilter Visit(ArrayFilter filter);
}