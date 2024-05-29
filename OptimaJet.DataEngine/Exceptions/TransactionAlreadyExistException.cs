namespace OptimaJet.DataEngine.Exceptions;

public class TransactionAlreadyExistException : InvalidOperationException
{
    const string DefaultMessage = "Transaction already exist.";
    
    internal TransactionAlreadyExistException(string? message = DefaultMessage) : base(message) {}
}