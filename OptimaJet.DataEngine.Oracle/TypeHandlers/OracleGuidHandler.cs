using System.Data;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;
using Oracle.ManagedDataAccess.Client;

namespace OptimaJet.DataEngine.Oracle.TypeHandlers;

public class OracleGuidHandler : GuidHandler
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        if (parameter is not OracleParameter oracleParameter)
        {
            throw new ArgumentException("The parameter must be an instance of OracleParameter", nameof(parameter));
        }
        
        oracleParameter.Value = value.ToString("N").ToUpperInvariant();
        oracleParameter.OracleDbType = OracleDbType.Char;
    }
}