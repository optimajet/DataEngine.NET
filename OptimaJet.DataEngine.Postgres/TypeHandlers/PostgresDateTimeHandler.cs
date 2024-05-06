using System.Data;
using Npgsql;
using NpgsqlTypes;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;

namespace OptimaJet.DataEngine.Postgres.TypeHandlers;

public class PostgresDateTimeHandler : DateTimeHandler
{
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        parameter.Value = value;
        parameter.DbType = DbType.DateTime2;

        switch (parameter)
        {
            case NpgsqlParameter postgres:
                postgres.NpgsqlDbType = NpgsqlDbType.Timestamp;
                break;
        }
    }
}
