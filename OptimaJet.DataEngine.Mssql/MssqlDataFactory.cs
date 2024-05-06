using Microsoft.Data.SqlClient;
using OptimaJet.DataEngine.Exceptions;
using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Mssql;

/// <summary>
/// Factory for implementing Mssql provider.
/// </summary>
public class MssqlDataFactory : SqlDataFactory
{
    /// <summary>
    /// Creates objects using default options and a new connection.
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    public MssqlDataFactory(string connectionString) 
        : this(new DataFactoryOptions(connectionString), new SqlConnection(connectionString), null) 
    {}
    
    /// <summary>
    /// Creates objects using specified options and a new connection.
    /// </summary>
    /// <param name="options">All options that are used to work with data.</param>
    public MssqlDataFactory(DataFactoryOptions options) 
        : this(options, new SqlConnection(options.DatabaseOptions.ConnectionString), null) 
    {}
    
    /// <summary>
    /// Creates objects using specified options, connection and transaction.
    /// Do not support nested transactions.
    /// </summary>
    /// <param name="options">All options that are used to work with data.</param>
    /// <param name="connection">Mssql connection, where you have begun a transaction</param>
    /// <param name="transaction">
    /// Allows you to specify an external transaction
    /// to pick it up and use it for further queries.
    /// </param>
    public MssqlDataFactory(DataFactoryOptions options, SqlConnection connection, SqlTransaction? transaction) 
        : base(options, connection, transaction) 
    {}

    public override IDatabase CreateDatabase()
    {
        _database = new MssqlDatabase(Options.DatabaseOptions, (SqlConnection) Connection, (SqlTransaction?) Transaction);
        return _database;
    }

    public override IDataSet<TEntity> CreateDataSet<TEntity>() where TEntity : class
    {
        if (_database == null) throw new MissingDatabaseException();
        
        return new MssqlDataSet<TEntity>(_database, Options.DataSetOptions);
    }

    private MssqlDatabase? _database;
}
