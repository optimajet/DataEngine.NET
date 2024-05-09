using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Oracle.Implementation;

internal class OracleDialect : Dialect
{
    protected override string LeftQuote => String.Empty;
    protected override string RightQuote => String.Empty;
}