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
        TypeHandlerRegistry.Register(new SqliteDateTimeHandler(), ProviderName.Sqlite);
        TypeHandlerRegistry.Register(new SqliteDateTimeOffsetHandler(), ProviderName.Sqlite);
        TypeHandlerRegistry.Register(new SqliteTimeSpanHandler(), ProviderName.Sqlite);
        TypeHandlerRegistry.Register(new SqliteGuidHandler(), ProviderName.Sqlite);
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