namespace OptimaJet.DataEngine.Exceptions;

internal class ProviderTypeNotSupportedException : NotSupportedException
{
    public ProviderTypeNotSupportedException(ProviderType type) : base($"Provider {type} not supported.") {}
}