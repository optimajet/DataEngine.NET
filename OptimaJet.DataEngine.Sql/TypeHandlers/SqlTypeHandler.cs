using System.Data;

namespace OptimaJet.DataEngine.Sql.TypeHandlers;

internal abstract class SqlTypeHandler<T> : ISqlTypeHandler
{
    public abstract void SetValue(IDbDataParameter parameter, T value);
    public abstract T Parse(object value);
    
    void ISqlTypeHandler.SetValue(IDbDataParameter parameter, object value)
    {
        SetValue(parameter, (T) value);
    }

    object? ISqlTypeHandler.Parse(object value)
    {
        return Parse(value);
    }
}