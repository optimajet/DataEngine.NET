using Dapper;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;

namespace OptimaJet.DataEngine.Sql.TypeHandlers;

public static class TypeHandlersPool
{
    static TypeHandlersPool()
    {
        CurrentProvider = String.Empty;
        _pool = new Dictionary<string, Dictionary<Type, ISqlTypeHandler>>();
    }
    
    public static string CurrentProvider { get; private set; }

    public static void SetCurrentProvider(string name)
    {
        if (name == CurrentProvider) return;

        var handlers = CreateAndGetTypeHandlers(name);
        
        SqlMapper.ResetTypeHandlers();
        
        RegisterTypeHandlers(handlers);

        CurrentProvider = name;
    }

    public static void SetTypeHandler(string providerName, Type type, ISqlTypeHandler handler)
    {
        var handlers = CreateAndGetTypeHandlers(providerName);
        handlers[type] = handler;
    }

    public static void SetTypeHandlers(string providerName, Dictionary<Type, ISqlTypeHandler> newHandlers)
    {
        var handlers = CreateAndGetTypeHandlers(providerName);

        foreach (var pair in newHandlers)
        {
            var type = pair.Key;
            var handler = pair.Value;
            
            handlers[type] = handler;
        }
    }
    
    private static Dictionary<Type, ISqlTypeHandler> DefaultTypeHandlers { get; } = new()
    {
        {typeof(byte), new ByteHandler()},
        {typeof(byte?), new ByteHandler()},
        {typeof(long), new Int64Handler()},
        {typeof(long?), new Int64Handler()},
        {typeof(float), new SingleHandler()},
        {typeof(float?), new SingleHandler()},
        {typeof(bool), new BooleanHandler()},
        {typeof(bool?), new BooleanHandler()},
        {typeof(DateTime), new DateTimeHandler()},
        {typeof(DateTime?), new DateTimeHandler()},
        {typeof(DateTimeOffset), new DateTimeOffsetHandler()},
        {typeof(DateTimeOffset?), new DateTimeOffsetHandler()},
        {typeof(TimeSpan), new TimeSpanHandler()},
        {typeof(TimeSpan?), new TimeSpanHandler()},
        {typeof(Guid), new GuidHandler()},
        {typeof(Guid?), new GuidHandler()},
    };
    
    private static readonly Dictionary<string, Dictionary<Type, ISqlTypeHandler>> _pool;

    private static Dictionary<Type, ISqlTypeHandler> CreateAndGetTypeHandlers(string name)
    {
        if (_pool.ContainsKey(name))
        {
            return _pool[name];
        }

        _pool[name] = new Dictionary<Type, ISqlTypeHandler>(DefaultTypeHandlers);

        return _pool[name];
    }
    
    private static void RegisterTypeHandlers(Dictionary<Type, ISqlTypeHandler> handlers)
    {
        foreach (var handler in handlers)
        {
            RegisterTypeHandler(handler.Key, new TypeHandlerAdapter(handler.Value));
        }
    }

    private static void RegisterTypeHandler(Type type, SqlMapper.ITypeHandler handler)
    {
        SqlMapper.RemoveTypeMap(type);
        SqlMapper.AddTypeHandler(type, handler);
    }
}