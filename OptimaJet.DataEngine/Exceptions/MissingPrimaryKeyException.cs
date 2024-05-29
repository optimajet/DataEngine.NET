namespace OptimaJet.DataEngine.Exceptions;

public class MissingPrimaryKeyException : InvalidOperationException
{
    const string DefaultMessage = "Entity does not have a primary key.";
    
    internal MissingPrimaryKeyException(string? message = DefaultMessage) : base(message) {}
}