namespace OptimaJet.DataEngine.Exceptions;

public class ProviderNotSupportedException : NotSupportedException
{
    internal ProviderNotSupportedException(string name) : base($"Provider {name} not supported.") {}
}