using System.Data;
using MySql.Data.MySqlClient;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;

namespace OptimaJet.DataEngine.Mysql.TypeHandlers;

internal class MysqlDateTimeOffsetHandler : DateTimeOffsetHandler
{
    public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
    {
        parameter.Value = value.ToUniversalTime();
        parameter.DbType = DbType.DateTimeOffset;

        switch (parameter)
        {
            case MySqlParameter mysql:
                mysql.Value = value.UtcDateTime;
                mysql.MySqlDbType = MySqlDbType.DateTime;
                break;
        }
    }
}
