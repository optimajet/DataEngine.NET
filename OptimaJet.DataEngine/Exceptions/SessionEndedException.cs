namespace OptimaJet.DataEngine.Exceptions;

public class SessionEndedException : InvalidOperationException
{
    const string DefaultMessage = "Session has been ended, no operations can be performed.";

    internal SessionEndedException(string? message = DefaultMessage) : base(message) { }
}