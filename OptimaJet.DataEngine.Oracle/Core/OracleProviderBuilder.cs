using OptimaJet.DataEngine.Sql;
using Oracle.ManagedDataAccess.Client;

namespace OptimaJet.DataEngine.Oracle;

public class OracleProviderBuilder : SqlProviderBuilder
{
    public OracleProviderBuilder(string connectionString, bool isUniqueInstance)
        : base(new OracleImplementation(), new SessionOptions(connectionString), isUniqueInstance)
    {
    }
    
    public OracleProviderBuilder(OracleConnection externalConnection, bool isUniqueInstance)
        : base(new OracleImplementation(), new SessionOptions(externalConnection), isUniqueInstance)
    {
    }
}