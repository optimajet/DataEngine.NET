namespace OptimaJet.DataEngine.Exceptions;

internal class DataContextCreationException : InvalidOperationException
{
    const string DefaultMessage = "There is no suitable constructor to create the DataContext.";
    
    public DataContextCreationException(string? message = DefaultMessage) : base(message) {}
}