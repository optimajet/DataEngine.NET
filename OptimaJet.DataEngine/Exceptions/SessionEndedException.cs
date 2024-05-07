namespace OptimaJet.DataEngine.Exceptions;

public class SessionEndedException : InvalidOperationException
{
    public SessionEndedException() : base("Session has been ended, no operations can be performed.") { }
}