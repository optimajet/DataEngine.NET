using System.Data;
using Microsoft.Data.Sqlite;
using OptimaJet.DataEngine.Sql.TypeHandlers;

namespace OptimaJet.DataEngine.Sqlite.TypeHandlers;

public class SqliteGuidHandler : GuidHandler
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        if (parameter is not SqliteParameter sqLiteParameter)
        {
            throw new ArgumentException("The parameter must be a SQLiteParameter.", nameof(parameter));
        }
        
        sqLiteParameter.DbType = DbType.String;
        sqLiteParameter.Value = value.ToString();
    }
}