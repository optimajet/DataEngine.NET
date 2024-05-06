using System.Data;

namespace OptimaJet.DataEngine.Sql.TypeHandlers;

public interface ISqlTypeHandler
{
    void SetValue(IDbDataParameter parameter, object value);
    object? Parse(object value);
}