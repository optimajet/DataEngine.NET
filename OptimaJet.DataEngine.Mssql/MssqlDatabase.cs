using Microsoft.Data.SqlClient;
using OptimaJet.DataEngine.Mssql.TypeHandlers;
using OptimaJet.DataEngine.Sql;
using OptimaJet.DataEngine.Sql.TypeHandlers;
using SqlKata.Compilers;

namespace OptimaJet.DataEngine.Mssql;

public class MssqlDatabase : SqlDatabase
{
    static MssqlDatabase()
    {
        TypeHandlersPool.SetTypeHandlers(ProviderType.Mssql, MssqlTypeHandlers);
    }
    
    /// <summary>
    /// Initializes static constructor
    /// </summary>
    public static void Activate() {}
    
    public MssqlDatabase(DatabaseOptions options, SqlConnection connection, SqlTransaction? transaction = null)
        : base(options, new SqlServerCompiler(), connection, transaction)
    { }
    
    public override ProviderType ProviderType => ProviderType.Mssql;
    
    private static Dictionary<Type, ISqlTypeHandler> MssqlTypeHandlers { get; } = new()
    {
        {typeof(TimeSpan), new MssqlTimeSpanHandler()},
        {typeof(TimeSpan?), new MssqlTimeSpanHandler()},
    };
}
