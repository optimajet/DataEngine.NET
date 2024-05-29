namespace OptimaJet.DataEngine.Exceptions;

public class ExpressionTreeParsingException : InvalidOperationException
{
    const string DefaultMessage = "An error occurred while parsing the expression tree.";
    
    internal ExpressionTreeParsingException(string? message = DefaultMessage) : base(message) {}
}