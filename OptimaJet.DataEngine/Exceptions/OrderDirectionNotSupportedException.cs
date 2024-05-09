namespace OptimaJet.DataEngine.Exceptions;

public class OrderDirectionNotSupportedException : NotSupportedException
{
    internal OrderDirectionNotSupportedException(string direction) : base($"Order direction {direction} not supported.") {}
}