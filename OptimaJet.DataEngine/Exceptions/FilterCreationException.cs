namespace OptimaJet.DataEngine.Exceptions;

public class FilterCreationException : InvalidOperationException
{
    const string DefaultMessage = "Error creating filter.";
    
    internal FilterCreationException(string? message = DefaultMessage) : base(message) {}
}