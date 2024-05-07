namespace OptimaJet.DataEngine.Exceptions;

internal class ProviderNotSupportedException : NotSupportedException
{
    public ProviderNotSupportedException(string name) : base($"Provider {name} not supported.") {}
}