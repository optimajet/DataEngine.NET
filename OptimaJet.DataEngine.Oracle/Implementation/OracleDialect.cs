using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Oracle.Implementation;

internal class OracleDialect : Dialect
{
    public override int MaxInClauseItems => 1000;

    protected override string LeftQuote => String.Empty;
    protected override string RightQuote => String.Empty;
}