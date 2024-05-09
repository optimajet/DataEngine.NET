namespace OptimaJet.DataEngine.Exceptions;

public class NullExpressionException : InvalidOperationException
{
    const string DefaultMessage = "There is no expression to create a constant..";
    
    internal NullExpressionException(string? message = DefaultMessage) : base(message) {}
}