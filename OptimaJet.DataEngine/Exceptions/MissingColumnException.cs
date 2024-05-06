namespace OptimaJet.DataEngine.Exceptions;

internal class MissingColumnException : InvalidOperationException
{
    public MissingColumnException(string name) : base($"Column {name} not found.") {}
}