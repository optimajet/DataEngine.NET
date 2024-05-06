using System.Diagnostics.Contracts;
using OptimaJet.DataEngine.Exceptions;

namespace OptimaJet.DataEngine.Sql.Glossaries;

public abstract class Dialect
{
    protected Dialect() {}

    public static Dialect Get(ProviderType type)
    {
        return Dialects.TryGetValue(type, out var glossary)
            ? glossary
            : throw new ProviderTypeNotSupportedException(type);
    }

    protected virtual string LeftQuote => "\"";
    protected virtual string RightQuote => "\"";
    
    [Pure]
    public virtual string Quote(string? @object) => $"{LeftQuote}{@object}{RightQuote}";

    private static readonly Dictionary<ProviderType, Dialect> Dialects = new()
    {
        [ProviderType.Mssql] = new MssqlDialect(),
        [ProviderType.Mysql] = new MysqlDialect(),
        [ProviderType.Oracle] = new OracleDialect(),
        [ProviderType.Postgres] = new PostgresDialect(),
        [ProviderType.Sqlite] = new SqliteDialect(),
    };
}