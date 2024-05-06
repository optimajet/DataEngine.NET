namespace OptimaJet.DataEngine.Exceptions;

internal class ContentTypeOutOfRangeException : ArgumentOutOfRangeException
{
    const string DefaultMessage = "Content type out of range.";
    
    public ContentTypeOutOfRangeException(string? message = DefaultMessage) : base(message) {}
}