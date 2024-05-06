namespace OptimaJet.DataEngine;

/// <summary>
/// All options that are used to work with data.
/// </summary>
public class DataFactoryOptions
{
    public DataFactoryOptions(string connectionString) : this(new DatabaseOptions(connectionString)) {}
    
    public DataFactoryOptions(
        DatabaseOptions databaseOptions,
        DataSetOptions? dataSetOptions = null,
        DataTransactionOptions? dataTransactionOptions = null)
    {
        DatabaseOptions = databaseOptions;
        DataSetOptions = dataSetOptions ?? new DataSetOptions();
        DataTransactionOptions = dataTransactionOptions ?? new DataTransactionOptions();
    }

    public DatabaseOptions DatabaseOptions { get; set; }
    public DataSetOptions DataSetOptions { get; set; }
    public DataTransactionOptions DataTransactionOptions { get; set; }
}