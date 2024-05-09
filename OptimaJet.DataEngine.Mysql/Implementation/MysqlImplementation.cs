using System.Data.Common;
using MySql.Data.MySqlClient;
using OptimaJet.DataEngine.Metadata;
using OptimaJet.DataEngine.Mysql.TypeHandlers;
using OptimaJet.DataEngine.Sql;
using OptimaJet.DataEngine.Sql.Implementation;
using OptimaJet.DataEngine.Sql.TypeHandlers;
using SqlKata.Compilers;

namespace OptimaJet.DataEngine.Mysql.Implementation;

internal class MysqlImplementation : ISqlImplementation
{
    static MysqlImplementation()
    {
        Dictionary<Type, ISqlTypeHandler> typeHandlers = new()
        {
            {typeof(DateTime), new MysqlDateTimeHandler()},
            {typeof(DateTime?), new MysqlDateTimeHandler()},
            {typeof(DateTimeOffset), new MysqlDateTimeOffsetHandler()},
            {typeof(DateTimeOffset?), new MysqlDateTimeOffsetHandler()},
        };

        TypeHandlersPool.SetTypeHandlers(ProviderName.Mysql, typeHandlers);
    }
    
    public string Name => ProviderName.Mysql;
    public Compiler Compiler { get; } = new MySqlCompiler();
    public Dialect Dialect { get; } = new MysqlDialect();
    
    public DbConnection CreateConnection(string connectionString)
    {
        return new MySqlConnection(connectionString);
    }

    public void ConfigureMetadata(EntityMetadata metadata)
    {
        metadata.GetNameFn ??= name => name.ToLowerInvariant();
    }
}