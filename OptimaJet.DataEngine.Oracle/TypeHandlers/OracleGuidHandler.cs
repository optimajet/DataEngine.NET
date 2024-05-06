using System.Data;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;
using Oracle.ManagedDataAccess.Client;

namespace OptimaJet.DataEngine.Oracle.TypeHandlers;

public class OracleGuidHandler : GuidHandler
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        if (parameter is OracleParameter oracle)
        {
            oracle.DbType = DbType.String;
            oracle.Value = value.ToString("N").ToUpperInvariant();
            oracle.OracleDbType = OracleDbType.Char;
            return;
        }
        
        parameter.DbType = DbType.Guid;
        parameter.Value = value;
    }
}
