namespace OptimaJet.DataEngine.Sql.Glossaries;

public class OracleDialect : Dialect
{
    protected override string LeftQuote => "";
    protected override string RightQuote => "";
}