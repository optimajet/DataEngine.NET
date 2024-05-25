using System.Data;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;
using Oracle.ManagedDataAccess.Client;

namespace OptimaJet.DataEngine.Oracle.TypeHandlers;

public class OracleTimeSpanHandler : TimeSpanHandler
{
    public override void SetValue(IDbDataParameter parameter, TimeSpan value)
    {
        if (parameter is not OracleParameter oracleParameter)
        {
            throw new ArgumentException("The parameter must be an instance of OracleParameter", nameof(parameter));
        }
        
        oracleParameter.Value = value;
        oracleParameter.OracleDbType = OracleDbType.IntervalDS;
    }
}