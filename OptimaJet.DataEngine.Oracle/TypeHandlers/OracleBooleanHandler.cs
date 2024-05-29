using System.Data;
using OptimaJet.DataEngine.Sql.TypeHandlers;
using Oracle.ManagedDataAccess.Client;

namespace OptimaJet.DataEngine.Oracle.TypeHandlers;

public class OracleBooleanHandler : BooleanHandler
{
    public override void SetValue(IDbDataParameter parameter, bool value)
    {
        if (parameter is not OracleParameter oracleParameter)
        {
            throw new ArgumentException("The parameter must be an instance of OracleParameter", nameof(parameter));
        }
        
        oracleParameter.Value = value switch
        {
            true => 1,
            false => 0
        };
        oracleParameter.OracleDbType = OracleDbType.Byte;
    }
}