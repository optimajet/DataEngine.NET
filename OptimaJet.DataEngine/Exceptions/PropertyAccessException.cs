namespace OptimaJet.DataEngine.Exceptions;

internal class PropertyAccessException : FieldAccessException
{
    const string DefaultMessage = "Entity property not available.";
    
    public PropertyAccessException(string? message = DefaultMessage) : base(message) {}
}