namespace OptimaJet.DataEngine.Exceptions;

public class ParentAlreadyExistException : InvalidOperationException
{
    const string DefaultMessage = "Parent already exist.";
    
    internal ParentAlreadyExistException(string? message = DefaultMessage) : base(message) {}
}