using System.Data;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;
using Oracle.ManagedDataAccess.Client;

namespace OptimaJet.DataEngine.Oracle.TypeHandlers;

internal class OracleBooleanHandler : BooleanHandler
{
    public override void SetValue(IDbDataParameter parameter, bool value)
    {
        parameter.DbType = DbType.Boolean;
        parameter.Value = value;
        
        switch (parameter)
        {
            case OracleParameter oracle:
                oracle.Value = value ? 1 : 0;
                oracle.DbType = DbType.Byte;
                oracle.OracleDbType = OracleDbType.Byte;
                break;
        }
    }
}
