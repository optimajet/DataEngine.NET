using System.Diagnostics.Contracts;

namespace OptimaJet.DataEngine.Sql;

public abstract class Dialect
{
    protected virtual string LeftQuote => "\"";
    protected virtual string RightQuote => "\"";
    
    [Pure]
    public virtual string Quote(string? @object) => $"{LeftQuote}{@object}{RightQuote}";
}