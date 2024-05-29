namespace OptimaJet.DataEngine.Exceptions;

public class DataContextCreationException : InvalidOperationException
{
    const string DefaultMessage = "There is no suitable constructor to create the DataContext.";
    
    internal DataContextCreationException(string? message = DefaultMessage) : base(message) {}
}