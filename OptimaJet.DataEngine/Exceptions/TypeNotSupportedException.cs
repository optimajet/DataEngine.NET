namespace OptimaJet.DataEngine.Exceptions;

public class TypeNotSupportedException : NotSupportedException
{
    const string DefaultMessage = "This type of entity property is not supported.";
    
    internal TypeNotSupportedException(string? message = DefaultMessage) : base(message) {}
}