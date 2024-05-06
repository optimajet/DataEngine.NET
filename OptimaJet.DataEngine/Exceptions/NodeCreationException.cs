namespace OptimaJet.DataEngine.Exceptions;

internal class NodeCreationException : ArgumentOutOfRangeException
{
    const string DefaultMessage = "It is not possible to create a child without specifying the node type.";
    
    public NodeCreationException(string? message = DefaultMessage) : base(message) {}
}