using MySqlConnector;

namespace OptimaJet.DataEngine.Mysql;

public static class MysqlProvider
{
    public static ProviderContext Use(string connectionString)
    {
        return ProviderContext.Use(new MysqlProviderBuilder(connectionString));
    }
    
    public static ProviderContext Use(MySqlConnection externalConnection)
    {
        return ProviderContext.Use(new MysqlProviderBuilder(externalConnection));
    }
    
    public static ProviderContext Create(string connectionString)
    {
        return ProviderContext.Use(new MysqlProviderBuilder(connectionString, true));
    }
    
    public static ProviderContext Create(MySqlConnection externalConnection)
    {
        return ProviderContext.Use(new MysqlProviderBuilder(externalConnection, true));
    }
}