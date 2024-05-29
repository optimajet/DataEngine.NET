using System.Collections.Concurrent;
using Dapper;

namespace OptimaJet.DataEngine.Sql.TypeHandlers;

public static class TypeHandlerRegistry
{
    static TypeHandlerRegistry()
    {
        RegisterDefaultGlobal(new ByteHandler());
        RegisterDefaultGlobal(new LongHandler());
        RegisterDefaultGlobal(new FloatHandler());
        RegisterDefaultGlobal(new BooleanHandler());
        RegisterDefaultGlobal(new DateTimeHandler());
        RegisterDefaultGlobal(new DateTimeOffsetHandler());
        RegisterDefaultGlobal(new TimeSpanHandler());
        RegisterDefaultGlobal(new GuidHandler());
    }
    
    internal static void RegisterDefaultGlobal<T>(SqlMapper.TypeHandler<T> handler)
    {
        RegisterDefaultGlobal<T>((SqlMapper.ITypeHandler) handler);
    }
    
    internal static void RegisterDefaultGlobal<T>(SqlMapper.ITypeHandler handler)
    {
        foreach (var providerName in AllProviderNames)
        {
            RegisterDefault<T>(handler, providerName);
        }
    }
    
    internal static void RegisterDefault<T>(SqlMapper.TypeHandler<T> handler, string providerName)
    {
        RegisterDefault<T>((SqlMapper.ITypeHandler) handler, providerName);
    }
    
    internal static void RegisterDefault<T>(SqlMapper.ITypeHandler handler, string providerName)
    {
        EnsureAdapterRegistered<T>();
        DefaultHandlers[GetTypeHandlerKey<T>(providerName)] = handler;
    }
    
    public static void Register<T>(SqlMapper.TypeHandler<T> handler, string providerName)
    {
        Register<T>((SqlMapper.ITypeHandler) handler, providerName);
    }
    
    public static void Register<T>(SqlMapper.ITypeHandler handler, string providerName)
    {
        EnsureAdapterRegistered<T>();
        Handlers[GetTypeHandlerKey<T>(providerName)] = handler;
    }

    public static void RegisterExternal<T>(SqlMapper.TypeHandler<T> handler)
    {
        RegisterExternal<T>((SqlMapper.ITypeHandler) handler);
    }

    public static void RegisterExternal<T>(SqlMapper.ITypeHandler handler)
    {
        EnsureAdapterRegistered<T>();
        Handlers[GetTypeHandlerKey<T>(ExternalProviderName)] = handler;
    }

    public static SqlMapper.ITypeHandler? Get<T>(string providerName)
    {
        return Handlers.TryGetValue(GetTypeHandlerKey<T>(providerName), out var handler)
            ? handler
            : DefaultHandlers.TryGetValue(GetTypeHandlerKey<T>(providerName), out var defaultHandler)
                ? defaultHandler
                : null;
    }

    public static SqlMapper.ITypeHandler? GetExternal<T>()
    {
        return Handlers.TryGetValue(GetTypeHandlerKey<T>(ExternalProviderName), out var handler) ? handler : null;
    }
    
    private static Tuple<Type, string> GetTypeHandlerKey<T>(string providerName)
    {
        return new Tuple<Type, string>(typeof(T), providerName);
    }
    
    private static void EnsureAdapterRegistered<T>()
    {
        lock (Lock)
        {
            SqlMapper.RemoveTypeMap(typeof(T));
            SqlMapper.AddTypeHandler(typeof(T), new TypeHandlerAdapter<T>());
        }
    }
    
    private static readonly string[] AllProviderNames =
    [
        ProviderName.Mssql,
        ProviderName.Mysql,
        ProviderName.Oracle,
        ProviderName.Postgres,
        ProviderName.Sqlite
    ];
    
    private static readonly string ExternalProviderName = "External";
    
    private static readonly ConcurrentDictionary<Tuple<Type, string>, SqlMapper.ITypeHandler> Handlers = new();
    private static readonly ConcurrentDictionary<Tuple<Type, string>, SqlMapper.ITypeHandler> DefaultHandlers = new();

    private static readonly object Lock = new();
}