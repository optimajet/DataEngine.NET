using System.Data.Common;
using Npgsql;
using OptimaJet.DataEngine.Metadata;
using OptimaJet.DataEngine.Postgres.TypeHandlers;
using OptimaJet.DataEngine.Sql;
using OptimaJet.DataEngine.Sql.Implementation;
using OptimaJet.DataEngine.Sql.TypeHandlers;
using SqlKata.Compilers;

namespace OptimaJet.DataEngine.Postgres.Implementation;

internal class PostgresImplementation : ISqlImplementation
{
    static PostgresImplementation()
    {
        Dictionary<Type, ISqlTypeHandler> typeHandlers = new()
        {
            {typeof(DateTime), new PostgresDateTimeHandler()},
            {typeof(DateTime?), new PostgresDateTimeHandler()},
        };

        TypeHandlersPool.SetTypeHandlers(ProviderName.Postgres, typeHandlers);
    }

    public string Name => ProviderName.Postgres;
    public Compiler Compiler { get; } = new PostgresCompiler();
    public Dialect Dialect { get; } = new PostgresDialect();
    
    public DbConnection CreateConnection(string connectionString)
    {
        return new NpgsqlConnection(connectionString);
    }

    public void ConfigureMetadata(EntityMetadata metadata)
    {
        // Do nothing
    }
}