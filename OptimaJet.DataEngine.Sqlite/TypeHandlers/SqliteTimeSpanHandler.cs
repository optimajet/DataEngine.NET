using System.Data;
using System.Data.SQLite;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;

namespace OptimaJet.DataEngine.Sqlite.TypeHandlers;

public class SqliteTimeSpanHandler : TimeSpanHandler
{
    public override void SetValue(IDbDataParameter parameter, TimeSpan value)
    {
        parameter.Value = value;
        parameter.DbType = DbType.Time;
        
        switch (parameter)
        {
            case SQLiteParameter sqlite:
                sqlite.Value = value.Ticks;
                sqlite.DbType = DbType.Int64;
                break;
        }
    }
}
