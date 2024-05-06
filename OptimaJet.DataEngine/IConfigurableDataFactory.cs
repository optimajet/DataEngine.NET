namespace OptimaJet.DataEngine;

/// <summary>
/// An abstract factory interface through which you can control the settings for creating objects.
/// </summary>
public interface IConfigurableDataFactory : IDataFactory
{
    /// <summary>
    /// All options that are used to work with data.
    /// </summary>
    public DataFactoryOptions Options { get; set; }
}