using OptimaJet.DataEngine.Oracle.Implementation;
using OptimaJet.DataEngine.Sql;
using Oracle.ManagedDataAccess.Client;

namespace OptimaJet.DataEngine.Oracle;

public class OracleProviderBuilder : SqlProviderBuilder
{
    public OracleProviderBuilder(string connectionString, bool isUniqueInstance = false)
        : base(new OracleImplementation(), connectionString, isUniqueInstance)
    {
    }
    
    public OracleProviderBuilder(OracleConnection externalConnection, bool isUniqueInstance = false)
        : base(new OracleImplementation(), externalConnection, isUniqueInstance)
    {
    }
}