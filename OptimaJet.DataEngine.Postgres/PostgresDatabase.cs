using Npgsql;
using OptimaJet.DataEngine.Postgres.TypeHandlers;
using OptimaJet.DataEngine.Sql;
using OptimaJet.DataEngine.Sql.TypeHandlers;
using SqlKata.Compilers;

namespace OptimaJet.DataEngine.Postgres;

public class PostgresDatabase : SqlDatabase
{
    static PostgresDatabase()
    {
        TypeHandlersPool.SetTypeHandlers(ProviderType.Postgres, PostgresTypeHandlers);
    }
    
    /// <summary>
    /// Initializes static constructor
    /// </summary>
    public static void Activate() {}
    
    public PostgresDatabase(DatabaseOptions options, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
        : base(options, new PostgresCompiler(), connection, transaction)
    { }
    
    public override ProviderType ProviderType => ProviderType.Postgres;
    
    private static Dictionary<Type, ISqlTypeHandler> PostgresTypeHandlers { get; } = new()
    {
        {typeof(DateTime), new PostgresDateTimeHandler()},
        {typeof(DateTime?), new PostgresDateTimeHandler()},
    };
}
