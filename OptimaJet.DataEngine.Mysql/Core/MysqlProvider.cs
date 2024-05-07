using MySql.Data.MySqlClient;

namespace OptimaJet.DataEngine.Mysql;

public static class MysqlProvider
{
    public static ProviderContext Use(string connectionString)
    {
        return ProviderContext.Use(new MysqlProviderBuilder(connectionString, false));
    }
    
    public static ProviderContext Use(MySqlConnection externalConnection)
    {
        return ProviderContext.Use(new MysqlProviderBuilder(externalConnection, false));
    }
    
    public static ProviderContext UseDetached(string connectionString)
    {
        return ProviderContext.Use(new MysqlProviderBuilder(connectionString, true));
    }
    
    public static ProviderContext UseDetached(MySqlConnection externalConnection)
    {
        return ProviderContext.Use(new MysqlProviderBuilder(externalConnection, true));
    }
}