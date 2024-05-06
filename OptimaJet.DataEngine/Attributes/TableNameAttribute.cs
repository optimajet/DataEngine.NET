namespace OptimaJet.DataEngine;

/// <summary>
/// Specifies the entity's metadata TableName for DTO class.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class TableNameAttribute : Attribute
{
    public TableNameAttribute(string name)
    {
        Name = name;
    }
    
    public string Name { get; set; }
}