using Microsoft.Data.Sqlite;

namespace OptimaJet.DataEngine.Sqlite;

public static class SqliteProvider
{
    public static ProviderContext Use(string connectionString)
    {
        return ProviderContext.Use(new SqliteProviderBuilder(connectionString));
    }
    
    public static ProviderContext Use(SqliteConnection externalConnection)
    {
        return ProviderContext.Use(new SqliteProviderBuilder(externalConnection));
    }
    
    public static ProviderContext Create(string connectionString)
    {
        return ProviderContext.Use(new SqliteProviderBuilder(connectionString, true));
    }
    
    public static ProviderContext Create(SqliteConnection externalConnection)
    {
        return ProviderContext.Use(new SqliteProviderBuilder(externalConnection, true));
    }
}