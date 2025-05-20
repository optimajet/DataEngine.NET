using System.Data;
using MySqlConnector;
using OptimaJet.DataEngine.Sql.TypeHandlers;

namespace OptimaJet.DataEngine.Mysql.TypeHandlers;

public class MysqlDateTimeHandler : DateTimeHandler
{
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        if (parameter is not MySqlParameter mysqlParameter)
        {
            throw new ArgumentException("The parameter must be of type MySqlParameter", nameof(parameter));
        }
        
        mysqlParameter.Value = value;
        mysqlParameter.MySqlDbType = MySqlDbType.DateTime;
    }
}