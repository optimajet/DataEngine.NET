using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Mysql.Implementation;

internal class MysqlDialect : Dialect
{
    protected override string LeftQuote => "`";
    protected override string RightQuote => "`";
}