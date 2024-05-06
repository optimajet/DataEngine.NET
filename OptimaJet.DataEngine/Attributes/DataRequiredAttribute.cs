namespace OptimaJet.DataEngine;

/// <summary>
/// Specifies the entity's metadata that this field must be not null.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DataRequiredAttribute : Attribute
{
    
}