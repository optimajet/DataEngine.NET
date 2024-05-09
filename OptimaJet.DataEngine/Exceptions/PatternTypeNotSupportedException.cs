namespace OptimaJet.DataEngine.Exceptions;

public class PatternTypeNotSupportedException : NotSupportedException
{
    const string DefaultMessage = "Pattern type not supported.";
    
    internal PatternTypeNotSupportedException(string? message = DefaultMessage) : base(message) {}
}