﻿using System.Data;
using System.Data.SQLite;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;

namespace OptimaJet.DataEngine.Sqlite.TypeHandlers;

public class SqliteDateTimeOffsetHandler : DateTimeOffsetHandler
{
    public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
    {
        if (parameter is not SQLiteParameter sqLiteParameter)
        {
            throw new ArgumentException("The parameter must be a SQLiteParameter.", nameof(parameter));
        }
        
        sqLiteParameter.Value = value.UtcDateTime.Ticks;
        sqLiteParameter.DbType = DbType.Int64;
    }
}