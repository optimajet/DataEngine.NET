namespace OptimaJet.DataEngine.Exceptions;

public class ContentTypeOutOfRangeException : ArgumentOutOfRangeException
{
    const string DefaultMessage = "Content type out of range.";
    
    internal ContentTypeOutOfRangeException(string? message = DefaultMessage) : base(message) {}
}