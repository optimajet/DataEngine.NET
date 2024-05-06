using Npgsql;
using OptimaJet.DataEngine.Exceptions;
using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Postgres;

/// <summary>
/// Factory for implementing Postgres provider.
/// </summary>
public class PostgresDataFactory : SqlDataFactory
{
    /// <summary>
    /// Creates objects using default options and a new connection.
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    public PostgresDataFactory(string connectionString) 
        : this(new DataFactoryOptions(connectionString), new NpgsqlConnection(connectionString), null) 
    {}
    
    /// <summary>
    /// Creates objects using specified options and a new connection.
    /// </summary>
    /// <param name="options">All options that are used to work with data.</param>
    public PostgresDataFactory(DataFactoryOptions options) 
        : this(options, new NpgsqlConnection(options.DatabaseOptions.ConnectionString), null) 
    {}
    
    /// <summary>
    /// Creates objects using specified options, connection and transaction.
    /// Do not support nested transactions.
    /// </summary>
    /// <param name="options">All options that are used to work with data.</param>
    /// <param name="connection">Postgres connection, where you have begun a transaction</param>
    /// <param name="transaction">
    /// Allows you to specify an external transaction
    /// to pick it up and use it for further queries.
    /// </param>
    public PostgresDataFactory(DataFactoryOptions options, NpgsqlConnection connection, NpgsqlTransaction? transaction) 
        : base(options, connection, transaction) 
    {}

    public override IDatabase CreateDatabase()
    {
        _database = new PostgresDatabase(Options.DatabaseOptions, (NpgsqlConnection) Connection, (NpgsqlTransaction?) Transaction);
        return _database;
    }

    public override IDataSet<TEntity> CreateDataSet<TEntity>() where TEntity : class
    {
        if (_database == null) throw new MissingDatabaseException();
        
        return new PostgresDataSet<TEntity>(_database, Options.DataSetOptions);
    }

    private PostgresDatabase? _database;
}
