namespace OptimaJet.DataEngine.Exceptions;

public class PropertyAccessException : FieldAccessException
{
    const string DefaultMessage = "Entity property not available.";
    
    internal PropertyAccessException(string? message = DefaultMessage) : base(message) {}
}