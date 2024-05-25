using System.Diagnostics.Contracts;

namespace OptimaJet.DataEngine.Sql;

public abstract class Dialect
{
    public virtual int MaxQueryParameters => 10000;
    public virtual int MaxInClauseItems => 10000;

    protected virtual string LeftQuote => "\"";
    protected virtual string RightQuote => "\"";

    [Pure]
    public virtual string Quote(string? @object) => $"{LeftQuote}{@object}{RightQuote}";
}