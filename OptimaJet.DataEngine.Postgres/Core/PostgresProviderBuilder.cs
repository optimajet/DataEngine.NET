using Npgsql;
using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Postgres;

public class PostgresProviderBuilder : SqlProviderBuilder
{
    public PostgresProviderBuilder(string connectionString, bool isUniqueInstance)
        : base(new PostgresImplementation(), new SessionOptions(connectionString), isUniqueInstance)
    {
    }
    
    public PostgresProviderBuilder(NpgsqlConnection externalConnection, bool isUniqueInstance)
        : base(new PostgresImplementation(), new SessionOptions(externalConnection), isUniqueInstance)
    {
    }
}