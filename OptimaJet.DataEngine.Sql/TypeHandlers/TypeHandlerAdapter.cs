using System.Data;
using Dapper;

namespace OptimaJet.DataEngine.Sql.TypeHandlers;

internal class TypeHandlerAdapter : SqlMapper.ITypeHandler
{
    public TypeHandlerAdapter(ISqlTypeHandler adaptee)
    {
        _adaptee = adaptee;
    }

    public void SetValue(IDbDataParameter parameter, object value)
    {
        _adaptee.SetValue(parameter, value);
    }

    public object? Parse(Type destinationType, object value)
    {
        return _adaptee.Parse(value);
    }
    
    private readonly ISqlTypeHandler _adaptee;
}