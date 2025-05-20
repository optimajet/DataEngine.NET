using System.Data.Common;
using Microsoft.Data.Sqlite;
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
        TypeHandlerRegistry.RegisterDefault(new SqliteDateTimeHandler(), ProviderName.Sqlite);
        TypeHandlerRegistry.RegisterDefault(new SqliteDateTimeOffsetHandler(), ProviderName.Sqlite);
        TypeHandlerRegistry.RegisterDefault(new SqliteTimeSpanHandler(), ProviderName.Sqlite);
        TypeHandlerRegistry.RegisterDefault(new SqliteGuidHandler(), ProviderName.Sqlite);
        TypeHandlerRegistry.RegisterDefault(new SqliteDecimalHandler(), ProviderName.Sqlite);
    }
    
    public string Name => ProviderName.Sqlite;
    public Compiler Compiler { get; } = new SqliteCompiler();
    public Dialect Dialect { get; } = new SqliteDialect();
    
    public DbConnection CreateConnection(string connectionString)
    {
        return new SqliteConnection(connectionString);
    }
    
    public void ConfigureMetadata(EntityMetadata metadata)
    {
        // Do nothing
    }
}