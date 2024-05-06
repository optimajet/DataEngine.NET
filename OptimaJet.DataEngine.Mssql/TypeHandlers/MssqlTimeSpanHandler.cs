using System.Data;
using Microsoft.Data.SqlClient;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;

namespace OptimaJet.DataEngine.Mssql.TypeHandlers;

public class MssqlTimeSpanHandler : TimeSpanHandler
{
    public override void SetValue(IDbDataParameter parameter, TimeSpan value)
    {
        parameter.Value = value;
        parameter.DbType = DbType.Time;

        switch (parameter)
        {
            case SqlParameter mssql:
                mssql.SqlDbType = SqlDbType.Time;
                break;
        }
    }
}
