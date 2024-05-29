namespace OptimaJet.DataEngine.Queries.Filters;

public abstract class Filter : IFilter
{
    protected Filter(IFilter[]? children = null)
    {
        Children = children ?? Array.Empty<IFilter>();

        foreach (var child in Children)
        {
            child.SetParent(this);
        }
    }

    public IFilter? Parent { get; private set; }
    public IFilter[] Children { get; }

    public void SetParent(IFilter parent)
    {
        Parent = parent;
    }

    public IFilter Reduce()
    {
        return FilterReducer.FilterReducer.Instance.Reduce(this);
    }

    public abstract FilterType FilterType { get; }
    public abstract IFilter Accept(IFilterVisitor visitor);
}