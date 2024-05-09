using System.Data.Common;
using OptimaJet.DataEngine.Metadata;
using OptimaJet.DataEngine.Oracle.TypeHandlers;
using OptimaJet.DataEngine.Sql;
using OptimaJet.DataEngine.Sql.Implementation;
using OptimaJet.DataEngine.Sql.TypeHandlers;
using Oracle.ManagedDataAccess.Client;
using SqlKata.Compilers;

namespace OptimaJet.DataEngine.Oracle.Implementation;

internal class OracleImplementation : ISqlImplementation
{
    static OracleImplementation()
    {
        Dictionary<Type, ISqlTypeHandler> typeHandlers = new()
        {
            {typeof(bool), new OracleBooleanHandler()},
            {typeof(bool?), new OracleBooleanHandler()},
            {typeof(DateTime), new OracleDateTimeHandler()},
            {typeof(DateTime?), new OracleDateTimeHandler()},
            {typeof(TimeSpan), new OracleTimeSpanHandler()},
            {typeof(TimeSpan?), new OracleTimeSpanHandler()},
            {typeof(Guid), new OracleGuidHandler()},
            {typeof(Guid?), new OracleGuidHandler()},
        };

        TypeHandlersPool.SetTypeHandlers(ProviderName.Oracle, typeHandlers);
    }
    
    public string Name => ProviderName.Oracle;
    public Compiler Compiler { get; } = new CustomOracleCompiler();
    public Dialect Dialect { get; } = new OracleDialect();
    
    public DbConnection CreateConnection(string connectionString)
    {
        return new OracleConnection(connectionString);
    }

    public void ConfigureMetadata(EntityMetadata metadata)
    {
        metadata.GetNameFn ??= name => name.ToUpperInvariant();

        foreach (var column in metadata.Columns)
        {
            column.GetNameFn ??= n => n.ToUpperInvariant();
        }
    }
}