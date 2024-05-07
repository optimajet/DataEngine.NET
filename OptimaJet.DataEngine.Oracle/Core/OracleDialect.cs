using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Oracle;

internal class OracleDialect : Dialect
{
    protected override string LeftQuote => String.Empty;
    protected override string RightQuote => String.Empty;
}