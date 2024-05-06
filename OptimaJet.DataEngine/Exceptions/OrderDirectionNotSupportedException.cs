namespace OptimaJet.DataEngine.Exceptions;

internal class OrderDirectionNotSupportedException : NotSupportedException
{
    public OrderDirectionNotSupportedException(string direction) : base($"Order direction {direction} not supported.") {}
}