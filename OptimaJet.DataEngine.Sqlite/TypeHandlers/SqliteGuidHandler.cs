using System.Data;
using System.Data.SQLite;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;

namespace OptimaJet.DataEngine.Sqlite.TypeHandlers;

internal class SqliteGuidHandler : GuidHandler
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        parameter.DbType = DbType.Guid;
        parameter.Value = value;

        switch (parameter)
        {
            case SQLiteParameter sqlite:
                sqlite.DbType = DbType.String;
                sqlite.Value = value.ToString();
                break;
        }
    }
}
