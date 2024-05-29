using Microsoft.Data.SqlClient;
using OptimaJet.DataEngine.Mssql.Implementation;
using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Mssql;

public class MssqlProviderBuilder : SqlProviderBuilder
{
    public MssqlProviderBuilder(string connectionString, bool isUniqueInstance = false)
        : base(new MssqlImplementation(), connectionString, isUniqueInstance)
    {
    }
    
    public MssqlProviderBuilder(SqlConnection externalConnection, bool isUniqueInstance = false)
        : base(new MssqlImplementation(), externalConnection, isUniqueInstance)
    {
    }
}