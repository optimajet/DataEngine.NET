using Dapper;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;

namespace OptimaJet.DataEngine.Sql.TypeHandlers;

public static class TypeHandlersPool
{
    static TypeHandlersPool()
    {
        CurrentProvider = ProviderType.Unspecified;
        _pool = new Dictionary<ProviderType, Dictionary<Type, ISqlTypeHandler>>();
    }
    
    public static ProviderType CurrentProvider { get; private set; }

    public static void SetCurrentProvider(ProviderType type)
    {
        if (type == CurrentProvider) return;

        var handlers = CreateAndGetTypeHandlers(type);
        
        SqlMapper.ResetTypeHandlers();
        
        RegisterTypeHandlers(handlers);

        CurrentProvider = type;
    }

    public static void SetTypeHandler(ProviderType providerType, Type type, ISqlTypeHandler handler)
    {
        var handlers = CreateAndGetTypeHandlers(providerType);
        handlers[type] = handler;
    }

    public static void SetTypeHandlers(ProviderType providerType, Dictionary<Type, ISqlTypeHandler> newHandlers)
    {
        var handlers = CreateAndGetTypeHandlers(providerType);

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
    
    private static readonly Dictionary<ProviderType, Dictionary<Type, ISqlTypeHandler>> _pool;

    private static Dictionary<Type, ISqlTypeHandler> CreateAndGetTypeHandlers(ProviderType type)
    {
        if (_pool.ContainsKey(type))
        {
            return _pool[type];
        }

        _pool[type] = new Dictionary<Type, ISqlTypeHandler>(DefaultTypeHandlers);

        return _pool[type];
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