using System.Data;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;
using Oracle.ManagedDataAccess.Client;

namespace OptimaJet.DataEngine.Oracle.TypeHandlers;

internal class OracleTimeSpanHandler : TimeSpanHandler
{
    public override void SetValue(IDbDataParameter parameter, TimeSpan value)
    {
        parameter.Value = value;
        parameter.DbType = DbType.Time;

        switch (parameter)
        {
            case OracleParameter oracle:
                oracle.Value = value;
                oracle.OracleDbType = OracleDbType.IntervalDS;
                break;
        }
    }
}
