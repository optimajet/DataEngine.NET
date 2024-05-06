namespace OptimaJet.DataEngine.Sql.Glossaries;

public class MysqlDialect : Dialect
{
    protected override string LeftQuote => "`";
    protected override string RightQuote => "`";
}