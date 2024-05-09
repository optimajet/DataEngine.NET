using System.Data.Common;
using System.Data.SQLite;
using OptimaJet.DataEngine.Metadata;
using OptimaJet.DataEngine.Sql;
using OptimaJet.DataEngine.Sql.Implementation;
using OptimaJet.DataEngine.Sql.TypeHandlers;
using OptimaJet.DataEngine.Sqlite.TypeHandlers;
using SqlKata.Compilers;

namespace OptimaJet.DataEngine.Sqlite.Implementation;

internal class SqliteImplementation : ISqlImplementation
{
    static SqliteImplementation()
    {
        Dictionary<Type, ISqlTypeHandler> typeHandlers = new()
        {
            {typeof(DateTime), new SqliteDateTimeHandler()},
            {typeof(DateTime?), new SqliteDateTimeHandler()},
            {typeof(DateTimeOffset), new SqliteDateTimeOffsetHandler()},
            {typeof(DateTimeOffset?), new SqliteDateTimeOffsetHandler()},
            {typeof(TimeSpan), new SqliteTimeSpanHandler()},
            {typeof(TimeSpan?), new SqliteTimeSpanHandler()},
            {typeof(Guid), new SqliteGuidHandler()},
            {typeof(Guid?), new SqliteGuidHandler()},
        };

        TypeHandlersPool.SetTypeHandlers(ProviderName.Sqlite, typeHandlers);
    }
    
    public string Name => ProviderName.Sqlite;
    public Compiler Compiler { get; } = new SqliteCompiler();
    public Dialect Dialect { get; } = new SqliteDialect();
    
    public DbConnection CreateConnection(string connectionString)
    {
        return new SQLiteConnection(connectionString);
    }

    public void ConfigureMetadata(EntityMetadata metadata)
    {
        // Do nothing
    }
}