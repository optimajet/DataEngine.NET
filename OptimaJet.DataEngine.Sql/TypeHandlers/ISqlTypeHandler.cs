using System.Data;

namespace OptimaJet.DataEngine.Sql.TypeHandlers;

internal interface ISqlTypeHandler
{
    void SetValue(IDbDataParameter parameter, object value);
    object? Parse(object value);
}