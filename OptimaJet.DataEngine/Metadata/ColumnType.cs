namespace OptimaJet.DataEngine.Metadata;

/// <summary>
/// Entity metadata column type 
/// </summary>
public class ColumnType
{
    public DataType Type { get; set; }
    public bool Nullable { get; set; }
    public bool Enumerable { get; set; }
    //Zero for max length
    public int Length { get; set; }
}