using System.Data;
using MySqlConnector;
using OptimaJet.DataEngine.Sql.TypeHandlers;

namespace OptimaJet.DataEngine.Mysql.TypeHandlers;

public class MysqlDateTimeOffsetHandler : DateTimeOffsetHandler
{
    public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
    {
        if (parameter is not MySqlParameter mysqlParameter)
        {
            throw new ArgumentException("The parameter must be a MySqlParameter.", nameof(parameter));
        }
        
        mysqlParameter.Value = value.UtcDateTime;
        mysqlParameter.MySqlDbType = MySqlDbType.DateTime;
    }
}