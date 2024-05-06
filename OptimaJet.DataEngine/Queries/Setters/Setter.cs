using OptimaJet.DataEngine.Metadata;

namespace OptimaJet.DataEngine.Setters;

public class Setter
{
    public Setter()
    {
        Fields = new Dictionary<string, object?>();
    }

    public Dictionary<string, object?> Fields { get; }

    public Dictionary<string, object?> GetNamedFields(EntityMetadata metadata)
    {
        return Fields.ToDictionary(pair => metadata.GetColumnNameByProperty(pair.Key), pair => pair.Value);
    }
}