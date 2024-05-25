using System.Collections.Concurrent;
using Dapper;
using OptimaJet.DataEngine.Sql.TypeHandlers.Default;

namespace OptimaJet.DataEngine.Sql.TypeHandlers;

public static class TypeHandlerRegistry
{
    static TypeHandlerRegistry()
    {
        RegisterAll(new ByteHandler());
        RegisterAll(new LongHandler());
        RegisterAll(new FloatHandler());
        RegisterAll(new BooleanHandler());
        RegisterAll(new DateTimeHandler());
        RegisterAll(new DateTimeOffsetHandler());
        RegisterAll(new TimeSpanHandler());
        RegisterAll(new GuidHandler());
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
    
    public static void RegisterAll<T>(SqlMapper.TypeHandler<T> handler)
    {
        Register<T>((SqlMapper.ITypeHandler) handler, AllProviderNames);
    }
    
    public static void RegisterAll<T>(SqlMapper.ITypeHandler handler)
    {
        Register<T>(handler, AllProviderNames);
    }
    
    public static void Register<T>(SqlMapper.TypeHandler<T> handler, params string[] providerNames)
    {
        Register<T>((SqlMapper.ITypeHandler) handler, providerNames);
    }
    
    public static void Register<T>(SqlMapper.ITypeHandler handler, params string[] providerNames)
    {
        EnsureAdapterRegistered<T>();
        
        foreach (var providerName in providerNames)
        {
            Handlers[GetTypeHandlerKey<T>(providerName)] = handler;
        }
    }
    
    public static SqlMapper.ITypeHandler? GetExternal<T>()
    {
        return Handlers.TryGetValue(GetTypeHandlerKey<T>(ExternalProviderName), out var handler) ? handler : null;
    }
    
    public static SqlMapper.ITypeHandler? Get<T>(string providerName)
    {
       return Handlers.TryGetValue(GetTypeHandlerKey<T>(providerName), out var handler) ? handler : null;
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
    
    private static readonly object Lock = new();
}