using OptimaJet.DataEngine.Oracle.TypeHandlers;
using OptimaJet.DataEngine.Sql;
using OptimaJet.DataEngine.Sql.TypeHandlers;
using Oracle.ManagedDataAccess.Client;

namespace OptimaJet.DataEngine.Oracle;

public class OracleDatabase : SqlDatabase
{
    static OracleDatabase()
    {
        TypeHandlersPool.SetTypeHandlers(ProviderType.Oracle, OracleTypeHandlers);
    }
    
    /// <summary>
    /// Initializes static constructor
    /// </summary>
    public static void Activate() {}
    
    public OracleDatabase(DatabaseOptions options, OracleConnection connection, OracleTransaction? transaction = null)
        : base(options, new CustomOracleCompiler(), connection, transaction)
    { }
    
    public override ProviderType ProviderType => ProviderType.Oracle;
    
    private static Dictionary<Type, ISqlTypeHandler> OracleTypeHandlers { get; } = new()
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
}
