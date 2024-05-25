using Microsoft.Data.SqlClient;

namespace OptimaJet.DataEngine.Mssql;

public static class MssqlProvider
{
    public static ProviderContext Use(string connectionString)
    {
        return ProviderContext.Use(new MssqlProviderBuilder(connectionString, false));
    }
    
    public static ProviderContext Use(SqlConnection externalConnection)
    {
        return ProviderContext.Use(new MssqlProviderBuilder(externalConnection, false));
    }
    
    public static ProviderContext Create(string connectionString)
    {
        return ProviderContext.Use(new MssqlProviderBuilder(connectionString, true));
    }
    
    public static ProviderContext Create(SqlConnection externalConnection)
    {
        return ProviderContext.Use(new MssqlProviderBuilder(externalConnection, true));
    }
}