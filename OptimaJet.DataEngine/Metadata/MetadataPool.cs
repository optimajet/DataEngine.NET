using System.Collections.Concurrent;

// ReSharper disable StaticMemberInGenericType

namespace OptimaJet.DataEngine.Metadata;

public static class MetadataPool<TEntity> where TEntity : class
{
    private static readonly ConcurrentDictionary<string, EntityMetadata> Pool = new();

    public static EntityMetadata GetMetadata(string name)
    {
        return Pool.GetOrAdd(name, _ => MetadataBuilder.Build<TEntity>());
    }
}