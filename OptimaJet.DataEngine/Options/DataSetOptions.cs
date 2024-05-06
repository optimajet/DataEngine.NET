namespace OptimaJet.DataEngine;

/// <summary>
/// Options for configuring data set processes
/// </summary>
public class DataSetOptions
{
    /// <summary>
    /// Schema name for dataset table
    /// </summary>
    public string? DatasetSchemaName { get; set; }
    public int? DatasetDefaultTimeout { get; set; }
}