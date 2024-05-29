namespace OptimaJet.DataEngine.Exceptions;

public class MissingColumnException : InvalidOperationException
{
    internal MissingColumnException(string name) : base($"Column {name} not found.") {}
}