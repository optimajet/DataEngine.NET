namespace OptimaJet.DataEngine.Exceptions;

internal class TransactionAlreadyExistException : InvalidOperationException
{
    const string DefaultMessage = "Transaction already exist.";
    
    public TransactionAlreadyExistException(string? message = DefaultMessage) : base(message) {}
}