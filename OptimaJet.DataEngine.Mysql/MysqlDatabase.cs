using MySql.Data.MySqlClient;
using OptimaJet.DataEngine.Mysql.TypeHandlers;
using OptimaJet.DataEngine.Sql;
using OptimaJet.DataEngine.Sql.TypeHandlers;
using SqlKata.Compilers;

namespace OptimaJet.DataEngine.Mysql;

public class MysqlDatabase : SqlDatabase
{
    static MysqlDatabase()
    {
        TypeHandlersPool.SetTypeHandlers(ProviderType.Mysql, MysqlTypeHandlers);
    }
    
    /// <summary>
    /// Initializes static constructor
    /// </summary>
    public static void Activate() {}
    
    public MysqlDatabase(DatabaseOptions options, MySqlConnection connection, MySqlTransaction? transaction = null)
        : base(options, new MySqlCompiler(), connection, transaction)
    { }
    
    public override ProviderType ProviderType => ProviderType.Mysql;
    
    private static Dictionary<Type, ISqlTypeHandler> MysqlTypeHandlers { get; } = new()
    {
        {typeof(DateTime), new MysqlDateTimeHandler()},
        {typeof(DateTime?), new MysqlDateTimeHandler()},
        {typeof(DateTimeOffset), new MysqlDateTimeOffsetHandler()},
        {typeof(DateTimeOffset?), new MysqlDateTimeOffsetHandler()},
    };
}
