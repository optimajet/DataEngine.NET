

// ReSharper disable PropertyCanBeMadeInitOnly.Global

using OptimaJet.DataEngine.Exceptions;

namespace OptimaJet.DataEngine.Metadata;

/// <summary>
/// Class that describes the metadata of a single entity column in a dataset
/// </summary>
public class EntityColumn
{
    public EntityColumn(string name, Type type)
    {
        OriginalName = name;
        RootType = type;
        Type = new ColumnType();
        Constraints = new List<ColumnConstraint>();
    }

    public string OriginalName { get; }
    public string Name => GetNameFn == null ? OriginalName : GetNameFn(OriginalName);
    public Func<string, string>? GetNameFn { get; set; }
    public ColumnType Type { get; set; }
    public Type RootType { get; }
    public List<ColumnConstraint> Constraints { get; set; }
    public bool IsPrimaryKey => Constraints.Any(c => c.Type == ConstraintType.PrimaryKey);

    public object? GetValue<TEntity>(TEntity entity) where TEntity : class
    {
        var type = typeof(TEntity);
        var getMethod = type.GetProperties().FirstOrDefault(p => p.Name == OriginalName)?.GetMethod;

        if (getMethod == null)
        {
            throw new PropertyAccessException($"Property {OriginalName} of entity {type.Name} is unreadable.");
        }

        var value = getMethod.Invoke(entity, Array.Empty<object?>());
        return value;
    }

    public void SetValue<TEntity>(TEntity entity, object? value)
    {
        var type = typeof(TEntity);
        var setMethod = type.GetProperties().FirstOrDefault(p => p.Name == OriginalName)?.SetMethod;

        if (setMethod == null)
        {
            throw new PropertyAccessException($"Property {OriginalName} of entity {type.Name} is not writable.");
        }

        setMethod.Invoke(entity, new [] {value});
    }
}