namespace OptimaJet.DataEngine.Exceptions;

internal class FilterCreationException : InvalidOperationException
{
    const string DefaultMessage = "Error creating filter.";
    
    public FilterCreationException(string? message = DefaultMessage) : base(message) {}
}