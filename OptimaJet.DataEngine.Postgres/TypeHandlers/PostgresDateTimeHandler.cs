using System.Data;
using Npgsql;
using NpgsqlTypes;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;

namespace OptimaJet.DataEngine.Postgres.TypeHandlers;

public class PostgresDateTimeHandler : DateTimeHandler
{
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        if (parameter is not NpgsqlParameter postgresParameter)
        {
            throw new ArgumentException("The parameter must be of type NpgsqlParameter", nameof(parameter));
        }
        
        postgresParameter.Value = value;
        postgresParameter.NpgsqlDbType = NpgsqlDbType.Timestamp;
    }
}