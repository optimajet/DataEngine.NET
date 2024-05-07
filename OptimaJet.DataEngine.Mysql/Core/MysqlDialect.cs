using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Mysql;

internal class MysqlDialect : Dialect
{
    protected override string LeftQuote => "`";
    protected override string RightQuote => "`";
}