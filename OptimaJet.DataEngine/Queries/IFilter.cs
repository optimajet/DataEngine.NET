namespace OptimaJet.DataEngine.Queries;

public interface IFilter
{
    IFilter? Parent { get; }
    IFilter[] Children { get; }
    FilterType FilterType { get; }

    IFilter Accept(IFilterVisitor visitor);
    void SetParent(IFilter parent);
    IFilter Reduce();
}