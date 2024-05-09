namespace OptimaJet.DataEngine.Exceptions;

public class MissingDatabaseException : InvalidOperationException
{
    const string DefaultMessage = "It isn't possible to create a DataSet before the database is created.";
    
    internal MissingDatabaseException(string? message = DefaultMessage) : base(message) {}
}