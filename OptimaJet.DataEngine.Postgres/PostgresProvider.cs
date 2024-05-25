using Npgsql;

namespace OptimaJet.DataEngine.Postgres;

public static class PostgresProvider
{
    public static ProviderContext Use(string connectionString)
    {
        return ProviderContext.Use(new PostgresProviderBuilder(connectionString, false));
    }
    
    public static ProviderContext Use(NpgsqlConnection externalConnection)
    {
        return ProviderContext.Use(new PostgresProviderBuilder(externalConnection, false));
    }
    
    public static ProviderContext Create(string connectionString)
    {
        return ProviderContext.Use(new PostgresProviderBuilder(connectionString, true));
    }
    
    public static ProviderContext Create(NpgsqlConnection externalConnection)
    {
        return ProviderContext.Use(new PostgresProviderBuilder(externalConnection, true));
    }
}