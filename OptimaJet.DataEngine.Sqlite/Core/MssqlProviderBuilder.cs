using System.Data.SQLite;
using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Sqlite;

public class SqliteProviderBuilder : SqlProviderBuilder
{
    public SqliteProviderBuilder(string connectionString, bool isUniqueInstance)
        : base(new SqliteImplementation(), new SessionOptions(connectionString), isUniqueInstance)
    {
    }
    
    public SqliteProviderBuilder(SQLiteConnection externalConnection, bool isUniqueInstance)
        : base(new SqliteImplementation(), new SessionOptions(externalConnection), isUniqueInstance)
    {
    }
}