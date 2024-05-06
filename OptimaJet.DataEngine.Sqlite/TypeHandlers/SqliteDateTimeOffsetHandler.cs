using System.Data;
using System.Data.SQLite;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;

namespace OptimaJet.DataEngine.Sqlite.TypeHandlers;

public class SqliteDateTimeOffsetHandler : DateTimeOffsetHandler
{
    public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
    {
        parameter.Value = value.ToUniversalTime();
        parameter.DbType = DbType.DateTimeOffset;

        switch (parameter)
        {
            case SQLiteParameter sqlite:
                sqlite.Value = value.UtcDateTime.Ticks;
                sqlite.DbType = DbType.Int64;
                break;
        }
    }
}
