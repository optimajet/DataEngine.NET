namespace OptimaJet.DataEngine.Exceptions;

internal class MissingPrimaryKeyException : InvalidOperationException
{
    const string DefaultMessage = "Entity does not have a primary key.";
    
    public MissingPrimaryKeyException(string? message = DefaultMessage) : base(message) {}
}