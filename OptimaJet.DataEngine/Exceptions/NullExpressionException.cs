namespace OptimaJet.DataEngine.Exceptions;

internal class NullExpressionException : InvalidOperationException
{
    const string DefaultMessage = "There is no expression to create a constant..";
    
    public NullExpressionException(string? message = DefaultMessage) : base(message) {}
}