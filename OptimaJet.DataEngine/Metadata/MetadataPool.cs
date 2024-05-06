using System.Collections.Concurrent;

// ReSharper disable StaticMemberInGenericType

namespace OptimaJet.DataEngine.Metadata;

public static class MetadataPool<TEntity> where TEntity : class
{
    private static readonly ConcurrentDictionary<ProviderType, EntityMetadata> Pool = new();

    public static EntityMetadata GetMetadata(ProviderType type)
    {
        return Pool.GetOrAdd(type, _ => MetadataBuilder.Build<TEntity>());
    }
}