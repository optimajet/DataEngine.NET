namespace OptimaJet.DataEngine.Attributes;

/// <summary>
/// Specifies the entity's metadata column type data length.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DataLengthAttribute : Attribute
{
    public DataLengthAttribute(int length)
    {
        Length = length;
    }

    public int Length { get; set; }
}