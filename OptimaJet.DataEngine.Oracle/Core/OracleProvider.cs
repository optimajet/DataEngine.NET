using Oracle.ManagedDataAccess.Client;

namespace OptimaJet.DataEngine.Oracle;

public static class OracleProvider
{
    public static ProviderContext Use(string connectionString)
    {
        return ProviderContext.Use(new OracleProviderBuilder(connectionString, false));
    }
    
    public static ProviderContext Use(OracleConnection externalConnection)
    {
        return ProviderContext.Use(new OracleProviderBuilder(externalConnection, false));
    }
    
    public static ProviderContext UseDetached(string connectionString)
    {
        return ProviderContext.Use(new OracleProviderBuilder(connectionString, true));
    }
    
    public static ProviderContext UseDetached(OracleConnection externalConnection)
    {
        return ProviderContext.Use(new OracleProviderBuilder(externalConnection, true));
    }
}