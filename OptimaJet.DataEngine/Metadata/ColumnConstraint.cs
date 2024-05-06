// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace OptimaJet.DataEngine.Metadata;

/// <summary>
/// Entity metadata column constraint
/// </summary>
public class ColumnConstraint
{
    public ColumnConstraint(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
    public ConstraintType Type { get; set; }
}