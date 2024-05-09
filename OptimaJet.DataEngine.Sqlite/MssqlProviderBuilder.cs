using System.Data.SQLite;
using OptimaJet.DataEngine.Sql;
using OptimaJet.DataEngine.Sqlite.Implementation;

namespace OptimaJet.DataEngine.Sqlite;

public class SqliteProviderBuilder : SqlProviderBuilder
{
    public SqliteProviderBuilder(string connectionString, bool isUniqueInstance)
        : base(new SqliteImplementation(), connectionString, isUniqueInstance)
    {
    }
    
    public SqliteProviderBuilder(SQLiteConnection externalConnection, bool isUniqueInstance)
        : base(new SqliteImplementation(), externalConnection, isUniqueInstance)
    {
    }
}