namespace OptimaJet.DataEngine.Exceptions;

internal class ExpressionTreeParsingException : InvalidOperationException
{
    const string DefaultMessage = "An error occurred while parsing the expression tree.";
    
    public ExpressionTreeParsingException(string? message = DefaultMessage) : base(message) {}
}