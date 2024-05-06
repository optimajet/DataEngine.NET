namespace OptimaJet.DataEngine.Exceptions;

internal class MissingDatabaseException : InvalidOperationException
{
    const string DefaultMessage = "It isn't possible to create a DataSet before the database is created.";
    
    public MissingDatabaseException(string? message = DefaultMessage) : base(message) {}
}