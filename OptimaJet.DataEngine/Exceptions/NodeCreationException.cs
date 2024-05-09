namespace OptimaJet.DataEngine.Exceptions;

public class NodeCreationException : ArgumentOutOfRangeException
{
    const string DefaultMessage = "It is not possible to create a child without specifying the node type.";
    
    internal NodeCreationException(string? message = DefaultMessage) : base(message) {}
}