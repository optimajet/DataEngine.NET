using Oracle.ManagedDataAccess.Client;

namespace OptimaJet.DataEngine.Oracle;

public static class OracleProvider
{
    public static ProviderContext Use(string connectionString)
    {
        return ProviderContext.Use(new OracleProviderBuilder(connectionString));
    }
    
    public static ProviderContext Use(OracleConnection externalConnection)
    {
        return ProviderContext.Use(new OracleProviderBuilder(externalConnection));
    }
    
    public static ProviderContext Create(string connectionString)
    {
        return ProviderContext.Use(new OracleProviderBuilder(connectionString, true));
    }

    public static ProviderContext Create(OracleConnection externalConnection)
    {
        return ProviderContext.Use(new OracleProviderBuilder(externalConnection, true));
    }
}