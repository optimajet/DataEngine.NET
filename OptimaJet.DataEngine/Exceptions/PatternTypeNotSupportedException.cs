namespace OptimaJet.DataEngine.Exceptions;

internal class PatternTypeNotSupportedException : NotSupportedException
{
    const string DefaultMessage = "Pattern type not supported.";
    
    public PatternTypeNotSupportedException(string? message = DefaultMessage) : base(message) {}
}