namespace OptimaJet.DataEngine.Exceptions;

internal class ParentAlreadyExistException : InvalidOperationException
{
    const string DefaultMessage = "Parent already exist.";
    
    public ParentAlreadyExistException(string? message = DefaultMessage) : base(message) {}
}