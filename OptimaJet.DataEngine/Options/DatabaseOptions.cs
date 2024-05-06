namespace OptimaJet.DataEngine;

/// <summary>
/// Options for configuring database processes
/// </summary>
public class DatabaseOptions
{
    public DatabaseOptions(string connectionString)
    {
        ConnectionString = connectionString;
        GlobalDefaultTimeout = DefaultTimeout;
        QueryExceptionHandler = DefaultQueryExceptionHandler;
    }

    public const int DefaultTimeout = 30;
    public static readonly Func<Exception, Exception> DefaultQueryExceptionHandler = exception => exception;

    /// <summary>
    /// Database connection string
    /// </summary>
    public string ConnectionString { get; set; }
    
    /// <summary>
    /// Command execution timeout for all queries
    /// </summary>
    public int GlobalDefaultTimeout { get; set; }
    
    /// <summary>
    /// Called during the handling of an exception while executing a query
    /// </summary>
    public Func<Exception, Exception> QueryExceptionHandler { get; set; }
    
    /// <summary>
    /// Method for logging generated queries
    /// </summary>
    public Action<string>? LogQueryFn { get; set; }
}