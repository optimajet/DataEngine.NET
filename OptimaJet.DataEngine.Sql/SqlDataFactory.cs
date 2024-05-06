using System.Data;

namespace OptimaJet.DataEngine.Sql;

/// <summary>
/// An abstract base class that implements the common logic of all relative database factories.
/// </summary>
public abstract class SqlDataFactory : IConfigurableDataFactory
{
    protected SqlDataFactory(DataFactoryOptions options, IDbConnection connection, IDbTransaction? transaction = null)
    {
        Options = options;
        Connection = connection;
        Transaction = transaction;
    }

    public DataFactoryOptions Options { get; set; }
    public IDbConnection Connection { get; set; }
    public IDbTransaction? Transaction { get; set; }

    public abstract IDatabase CreateDatabase();
    public abstract IDataSet<TEntity> CreateDataSet<TEntity>() where TEntity : class;
}