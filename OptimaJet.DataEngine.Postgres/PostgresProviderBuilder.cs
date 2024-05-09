using Npgsql;
using OptimaJet.DataEngine.Postgres.Implementation;
using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Postgres;

public class PostgresProviderBuilder : SqlProviderBuilder
{
    public PostgresProviderBuilder(string connectionString, bool isUniqueInstance)
        : base(new PostgresImplementation(), connectionString, isUniqueInstance)
    {
    }
    
    public PostgresProviderBuilder(NpgsqlConnection externalConnection, bool isUniqueInstance)
        : base(new PostgresImplementation(), externalConnection, isUniqueInstance)
    {
    }
}