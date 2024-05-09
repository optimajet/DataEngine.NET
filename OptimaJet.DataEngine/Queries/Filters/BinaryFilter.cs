namespace OptimaJet.DataEngine.Queries.Filters;

public abstract class BinaryFilter : Filter
{
    protected BinaryFilter(IFilter left, IFilter right) : base(new[] {left, right})
    {
        Left = left;
        Right = right;
    }
    
    public IFilter Left { get; }
    public IFilter Right { get; }
}