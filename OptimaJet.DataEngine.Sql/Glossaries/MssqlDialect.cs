namespace OptimaJet.DataEngine.Sql.Glossaries;

public class MssqlDialect : Dialect
{
    protected override string LeftQuote => "[";
    protected override string RightQuote => "]";
}