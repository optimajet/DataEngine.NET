using System.Data;
using MySql.Data.MySqlClient;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;

namespace OptimaJet.DataEngine.Mysql.TypeHandlers;

internal class MysqlDateTimeHandler : DateTimeHandler
{
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        parameter.Value = value;
        parameter.DbType = DbType.DateTime2;

        switch (parameter)
        {
            case MySqlParameter mysql:
                mysql.MySqlDbType = MySqlDbType.DateTime;
                break;
        }
    }
}
