using System.Data;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;
using Oracle.ManagedDataAccess.Client;

namespace OptimaJet.DataEngine.Oracle.TypeHandlers;

public class OracleDateTimeHandler : DateTimeHandler
{
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        if (parameter is not OracleParameter oracleParameter)
        {
            throw new ArgumentException("The parameter must be of type OracleParameter", nameof(parameter));
        }
        
        oracleParameter.Value = value;
        oracleParameter.DbType = DbType.DateTime;
    }
}