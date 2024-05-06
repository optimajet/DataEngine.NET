namespace OptimaJet.DataEngine;

/// <summary>
/// An abstract factory for creating objects related to a single data provider.
/// </summary>
public interface IDataFactory
{
    /// <summary>
    /// Creates a Database object.
    /// Must be called before creating DataSet.
    /// </summary>
    /// <returns>Database object</returns>
    IDatabase CreateDatabase();
    
    /// <summary>
    /// Creates a DataSet object.
    /// Cannot be created before the database is created.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity/DTO class that the DataSet will manage.</typeparam>
    /// <returns>DataSet object</returns>
    IDataSet<TEntity> CreateDataSet<TEntity>() where TEntity : class;
}
