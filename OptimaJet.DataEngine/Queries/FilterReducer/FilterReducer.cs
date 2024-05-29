using OptimaJet.DataEngine.Queries.FilterBuilder;
using OptimaJet.DataEngine.Queries.Filters;

namespace OptimaJet.DataEngine.Queries.FilterReducer;

internal class FilterReducer : FilterVisitor, IFilterReducer
{
    private static readonly Lazy<IFilterReducer> LazyInstance = new(() => new FilterReducer());
    public static IFilterReducer Instance => LazyInstance.Value;

    public IFilter Reduce(IFilter filter)
    {
        return filter.Accept(this);
    }

    public override IFilter Visit(AndFilter filter)
    {
        var left = filter.Left.Accept(this);
        var right = filter.Right.Accept(this);

        return (left, right) switch
        {
            (TrueFilter _, TrueFilter _) => TrueFilter.Instance,
            (FalseFilter _, _) => FalseFilter.Instance,
            (_, FalseFilter _) => FalseFilter.Instance,
            (TrueFilter _, _) => right,
            (_, TrueFilter _) => left,
            _ when filter.Left != left || filter.Right != right => new AndFilter(left, right),
            _ => filter
        };
    }

    public override IFilter Visit(OrFilter filter)
    {
        var left = filter.Left.Accept(this);
        var right = filter.Right.Accept(this);

        return (left, right) switch
        {
            (FalseFilter _, FalseFilter _) => FalseFilter.Instance,
            (TrueFilter _, _) => TrueFilter.Instance,
            (_, TrueFilter _) => TrueFilter.Instance,
            (FalseFilter _, _) => right,
            (_, FalseFilter _) => left,
            _ when filter.Left != left || filter.Right != right => new OrFilter(left, right),
            _ => filter
        };
    }

    public override IFilter Visit(NotFilter filter)
    {
        var operand = filter.Operand.Accept(this);

        return operand switch
        {
            TrueFilter _ => FalseFilter.Instance,
            FalseFilter _ => TrueFilter.Instance,
            _ when filter.Operand != operand => new NotFilter(operand),
            _ => filter
        };
    }
}