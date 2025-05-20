using System.Data;
using Dapper;

namespace OptimaJet.DataEngine.Sqlite.TypeHandlers;

public class SqliteDecimalHandler : SqlMapper.TypeHandler<Decimal>
{
    public override void SetValue(IDbDataParameter parameter, Decimal value)
    {
        throw new NotSupportedException("Decimal values is not supported by the SQLite provider.");
    }
    
    public override Decimal Parse(object value)
    {
        throw new NotSupportedException("Decimal values is not supported by the SQLite provider.");
    }
}
