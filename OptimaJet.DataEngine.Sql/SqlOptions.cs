namespace OptimaJet.DataEngine.Sql;

public class SqlOptions : IOptions
{
    public string? DatabaseSchema { get; set; }
    public int CommandTimeout { get; set; } = 30;
    public Action<Exception> ExceptionHandler { get; set; } = _ => { };
    public Action<string> LogQueryAction { get; set; } = _ => { };

    public SqlOptions Clone()
    {
        return new SqlOptions
        {
            DatabaseSchema = DatabaseSchema,
            CommandTimeout = CommandTimeout,
            ExceptionHandler = ExceptionHandler,
            LogQueryAction = LogQueryAction
        };
    }

    object ICloneable.Clone()
    {
        return Clone();
    }
}