using System.Data;
using System.Globalization;
using Dapper;
using Type = System.Type;

namespace OptimaJet.DataEngine.Sql.TypeHandlers;

internal class TypeHandlerAdapter<T> : SqlMapper.ITypeHandler
{
    public void SetValue(IDbDataParameter parameter, object value)
    {
        var handler = GetHandler();
        
        if (handler != null)
        {
            handler.SetValue(parameter, value);
            return;
        }
        
        var dbType = DefaultTypeMap.Get(typeof(T));
        
        if (dbType == null)
        {
            throw new InvalidOperationException($"Type {typeof(T).Name} is not mapped to DbType in DefaultTypeMap");
        }
        
        parameter.Value = value;
        parameter.DbType = dbType.Value;
    }
    
    public object? Parse(Type destinationType, object value)
    {
        var handler = GetHandler();
        
        if (handler != null)
        {
            return handler.Parse(destinationType, value);
        }
        
        return Convert.ChangeType(value, destinationType, CultureInfo.InvariantCulture);
    }
    
    private static SqlMapper.ITypeHandler? GetHandler()
    {
        var provider = ProviderContext.CurrentOrNull;
        return provider == null
            ? TypeHandlerRegistry.GetExternal<T>()
            : TypeHandlerRegistry.Get<T>(provider.Name);
    }
}