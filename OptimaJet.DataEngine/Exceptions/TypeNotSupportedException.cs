namespace OptimaJet.DataEngine.Exceptions;

internal class TypeNotSupportedException : NotSupportedException
{
    const string DefaultMessage = "This type of entity property is not supported.";
    
    public TypeNotSupportedException(string? message = DefaultMessage) : base(message) {}
}