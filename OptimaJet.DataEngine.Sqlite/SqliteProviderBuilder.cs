using Microsoft.Data.Sqlite;
using OptimaJet.DataEngine.Sql;
using OptimaJet.DataEngine.Sqlite.Implementation;

namespace OptimaJet.DataEngine.Sqlite;

public class SqliteProviderBuilder : SqlProviderBuilder
{
    public SqliteProviderBuilder(string connectionString, bool isUniqueInstance = false)
        : base(new SqliteImplementation(), connectionString, isUniqueInstance)
    {
    }
    
    public SqliteProviderBuilder(SqliteConnection externalConnection, bool isUniqueInstance = false)
        : base(new SqliteImplementation(), externalConnection, isUniqueInstance)
    {
    }
}