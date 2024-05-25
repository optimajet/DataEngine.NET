using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Mssql.Implementation;

internal class MssqlDialect : Dialect
{
    public override int MaxQueryParameters => 2099;
    public override int MaxInClauseItems => 2048;

    protected override string LeftQuote => "[";
    protected override string RightQuote => "]";
}