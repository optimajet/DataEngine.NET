namespace OptimaJet.DataEngine.Exceptions;

public class NoAvailableProviderException : InvalidOperationException
{
    const string DefaultMessage = "No available provider, use ProviderContext.Use() " +
                                  "to use a provider in the current context.";

    internal NoAvailableProviderException(string? message = DefaultMessage) : base(message) {}
}