using System.Data;
using System.Data.SQLite;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;

namespace OptimaJet.DataEngine.Sqlite.TypeHandlers;

public class SqliteDateTimeHandler : DateTimeHandler
{
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        if (parameter is not SQLiteParameter sqLiteParameter)
        {
            throw new ArgumentException("The parameter must be of type SQLiteParameter", nameof(parameter));
        }
        
        sqLiteParameter.Value = value.Ticks;
        sqLiteParameter.DbType = DbType.Int64;
    }
}