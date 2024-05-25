using System.Data.SQLite;

namespace OptimaJet.DataEngine.Sqlite;

public static class SqliteProvider
{
    public static ProviderContext Use(string connectionString)
    {
        return ProviderContext.Use(new SqliteProviderBuilder(connectionString, false));
    }
    
    public static ProviderContext Use(SQLiteConnection externalConnection)
    {
        return ProviderContext.Use(new SqliteProviderBuilder(externalConnection, false));
    }
    
    public static ProviderContext Create(string connectionString)
    {
        return ProviderContext.Use(new SqliteProviderBuilder(connectionString, true));
    }
    
    public static ProviderContext Create(SQLiteConnection externalConnection)
    {
        return ProviderContext.Use(new SqliteProviderBuilder(externalConnection, true));
    }
}