using System.Data;
using Microsoft.Data.Sqlite;
using OptimaJet.DataEngine.Sql.TypeHandlers;

namespace OptimaJet.DataEngine.Sqlite.TypeHandlers;

public class SqliteDateTimeHandler : DateTimeHandler
{
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        if (parameter is not SqliteParameter sqLiteParameter)
        {
            throw new ArgumentException("The parameter must be of type SQLiteParameter", nameof(parameter));
        }
        
        sqLiteParameter.Value = value.Ticks;
        sqLiteParameter.DbType = DbType.Int64;
    }
}