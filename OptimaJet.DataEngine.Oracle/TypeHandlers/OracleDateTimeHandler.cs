using System.Data;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;
using Oracle.ManagedDataAccess.Client;

namespace OptimaJet.DataEngine.Oracle.TypeHandlers;

public class OracleDateTimeHandler : DateTimeHandler
{
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        if (parameter is OracleParameter oracle)
        {
            oracle.DbType = DbType.DateTime;
            oracle.Value = value;
            return;
        }
        
        parameter.Value = value;
        parameter.DbType = DbType.DateTime2;
    }
}
