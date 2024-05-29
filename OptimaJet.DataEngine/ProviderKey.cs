namespace OptimaJet.DataEngine;

public readonly struct ProviderKey
{
    public string ProviderName { get; }
    public object? ConnectionObject { get; }
    public string? ConnectionString { get; }
    public string? UniqueKey { get; }

    private ProviderKey(string providerName)
    {
        ProviderName = providerName;
        UniqueKey = Guid.NewGuid().ToString("N");
    }
    
    private ProviderKey(string providerName, object connectionObject)
    {
        ProviderName = providerName;
        ConnectionObject = connectionObject;
    }
    
    private ProviderKey(string providerName, string connectionString)
    {
        ProviderName = providerName;
        ConnectionString = connectionString;
    }
    
    public override bool Equals(object? obj)
    {
        return obj switch
        {
            ProviderKey {UniqueKey: not null} key => key.UniqueKey.Equals(UniqueKey),
            ProviderKey {ConnectionObject: not null} key => key.ProviderName.Equals(ProviderName) &&
                                                            key.ConnectionObject.Equals(ConnectionObject),
            ProviderKey {ConnectionString: not null} key => key.ProviderName.Equals(ProviderName) &&
                                                            key.ConnectionString.Equals(ConnectionString),
            _ => false
        };
    }
    
    public override int GetHashCode()
    {
        if (UniqueKey != null)
        {
            return UniqueKey.GetHashCode();
        }
        
        unchecked
        {
            var hashCode = ProviderName.GetHashCode();
            hashCode = (hashCode * 397) ^ (ConnectionObject != null ? ConnectionObject.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (ConnectionString != null ? ConnectionString.GetHashCode() : 0);
            return hashCode;
        }
    }
    
    public override string ToString()
    {
        List<string> parts = [$"Provider: {ProviderName}"];
        if (ConnectionObject != null)
        {
            parts.Add($"ConnectionObject: {ConnectionObject}");
        }
        
        if (ConnectionString != null)
        {
            parts.Add($"ConnectionString: {ConnectionString}");
        }
        
        if (!string.IsNullOrEmpty(UniqueKey))
        {
            parts.Add($"UniqueKey: {UniqueKey}");
        }
        
        return string.Join(", ", parts);
    }

    public static ProviderKey GetUniqueKey(string providerName)
    {
        return new ProviderKey(providerName);
    }
    
    public static ProviderKey GetKey(string providerName, object connectionObject)
    {
        return new ProviderKey(providerName, connectionObject);
    }
    
    public static ProviderKey GetKey(string providerName, string connectionString)
    {
        return new ProviderKey(providerName, connectionString);
    }
}