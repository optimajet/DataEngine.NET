using System.Data.SQLite;
using OptimaJet.DataEngine.Sql;
using OptimaJet.DataEngine.Sql.TypeHandlers;
using OptimaJet.DataEngine.Sqlite.TypeHandlers;
using SqlKata.Compilers;

namespace OptimaJet.DataEngine.Sqlite;

public class SqliteDatabase : SqlDatabase
{
    static SqliteDatabase()
    {
        TypeHandlersPool.SetTypeHandlers(ProviderType.Sqlite, SqliteTypeHandlers);
    }
    
    /// <summary>
    /// Initializes static constructor
    /// </summary>
    public static void Activate() {}
    
    public SqliteDatabase(DatabaseOptions options, SQLiteConnection connection, SQLiteTransaction? transaction = null)
        : base(options, new SqliteCompiler(), connection, transaction)
    { }
    
    public override ProviderType ProviderType => ProviderType.Sqlite;
    
    private static Dictionary<Type, ISqlTypeHandler> SqliteTypeHandlers { get; } = new()
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
}
