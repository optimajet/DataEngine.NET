namespace OptimaJet.DataEngine;

/// <summary>
/// A data context that binds all objects to work with data.
/// Works with a single database connection that opens automatically.
/// Used as a parent class for implementations of various data models.
/// In the child class, dataset properties are declared,
/// which are created using abstract factory.
/// </summary>
public class DataContext : IDisposable
{
    public DataContext(IDataFactory factory, DataContextOptions? options = null)
    {
        Factory = factory;
        Options = options ?? new DataContextOptions();
        Database = factory.CreateDatabase();
    }

    public IDatabase Database { get; }

    public void Dispose()
    {
        if (_disposed) return;
        
        Database.Dispose();
        _disposed = true;
    }

    protected IDataFactory Factory { get; }
    protected DataContextOptions Options { get; }

    private bool _disposed;
}
