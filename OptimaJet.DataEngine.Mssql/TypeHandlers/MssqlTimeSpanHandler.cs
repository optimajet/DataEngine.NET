using System.Data;
using Microsoft.Data.SqlClient;
using OptimaJet.DataEngine.Sql.TypeHandlers;

namespace OptimaJet.DataEngine.Mssql.TypeHandlers;

public class MssqlTimeSpanHandler : TimeSpanHandler
{
    public override void SetValue(IDbDataParameter parameter, TimeSpan value)
    {
        if (parameter is not SqlParameter sqlParameter)
        {
            throw new ArgumentException("The parameter must be a SqlParameter.", nameof(parameter));
        }
        
        sqlParameter.Value = value;
        sqlParameter.SqlDbType = SqlDbType.Time;
    }
}