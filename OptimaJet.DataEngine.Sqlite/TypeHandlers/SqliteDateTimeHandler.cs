using System.Data;
using System.Data.SQLite;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;

namespace OptimaJet.DataEngine.Sqlite.TypeHandlers;

public class SqliteDateTimeHandler : DateTimeHandler
{
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        parameter.Value = value;
        parameter.DbType = DbType.DateTime2;

        switch (parameter)
        {
            case SQLiteParameter sqlite:
                sqlite.Value = value.Ticks;
                sqlite.DbType = DbType.Int64;
                break;
        }
    }
}
