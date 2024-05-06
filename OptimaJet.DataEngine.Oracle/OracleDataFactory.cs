using OptimaJet.DataEngine.Exceptions;
using OptimaJet.DataEngine.Sql;
using Oracle.ManagedDataAccess.Client;

namespace OptimaJet.DataEngine.Oracle;

/// <summary>
/// Factory for implementing Oracle provider.
/// </summary>
public class OracleDataFactory : SqlDataFactory
{
    /// <summary>
    /// Creates objects using default options and a new connection.
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    public OracleDataFactory(string connectionString) 
        : this(new DataFactoryOptions(connectionString), new OracleConnection(connectionString), null) 
    {}
    
    /// <summary>
    /// Creates objects using specified options and a new connection.
    /// </summary>
    /// <param name="options">All options that are used to work with data.</param>
    public OracleDataFactory(DataFactoryOptions options) 
        : this(options, new OracleConnection(options.DatabaseOptions.ConnectionString), null) 
    {}
    
    /// <summary>
    /// Creates objects using specified options, connection and transaction.
    /// Do not support nested transactions.
    /// </summary>
    /// <param name="options">All options that are used to work with data.</param>
    /// <param name="connection">Oracle connection, where you have begun a transaction</param>
    /// <param name="transaction">
    /// Allows you to specify an external transaction
    /// to pick it up and use it for further queries.
    /// </param>
    public OracleDataFactory(DataFactoryOptions options, OracleConnection connection, OracleTransaction? transaction) 
        : base(options, connection, transaction) 
    {}

    public override IDatabase CreateDatabase()
    {
        _database = new OracleDatabase(Options.DatabaseOptions, (OracleConnection) Connection, (OracleTransaction?) Transaction);
        return _database;
    }

    public override IDataSet<TEntity> CreateDataSet<TEntity>() where TEntity : class
    {
        if (_database == null) throw new MissingDatabaseException();
        
        return new OracleDataSet<TEntity>(_database, Options.DataSetOptions);
    }

    private OracleDatabase? _database;
}
