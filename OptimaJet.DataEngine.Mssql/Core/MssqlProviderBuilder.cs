using Microsoft.Data.SqlClient;
using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Mssql;

public class MssqlProviderBuilder : SqlProviderBuilder
{
    public MssqlProviderBuilder(string connectionString, bool isUniqueInstance)
        : base(new MssqlImplementation(), new SessionOptions(connectionString), isUniqueInstance)
    {
    }
    
    public MssqlProviderBuilder(SqlConnection externalConnection, bool isUniqueInstance)
        : base(new MssqlImplementation(), new SessionOptions(externalConnection), isUniqueInstance)
    {
    }
    
    public MssqlProviderBuilder(SessionOptions options, bool isUniqueInstance)
        : base(new MssqlImplementation(), options, isUniqueInstance)
    {
    }
}