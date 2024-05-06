using MySql.Data.MySqlClient;
using OptimaJet.DataEngine.Exceptions;
using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Mysql;

/// <summary>
/// Factory for implementing Mysql provider.
/// </summary>
public class MysqlDataFactory : SqlDataFactory
{
    /// <summary>
    /// Creates objects using default options and a new connection.
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    public MysqlDataFactory(string connectionString) 
        : this(new DataFactoryOptions(connectionString), new MySqlConnection(connectionString), null) 
    {}
    
    /// <summary>
    /// Creates objects using specified options and a new connection.
    /// </summary>
    /// <param name="options">All options that are used to work with data.</param>
    public MysqlDataFactory(DataFactoryOptions options) 
        : this(options, new MySqlConnection(options.DatabaseOptions.ConnectionString), null) 
    {}
    
    /// <summary>
    /// Creates objects using specified options, connection and transaction.
    /// Do not support nested transactions.
    /// </summary>
    /// <param name="options">All options that are used to work with data.</param>
    /// <param name="connection">Mysql connection, where you have begun a transaction</param>
    /// <param name="transaction">
    /// Allows you to specify an external transaction
    /// to pick it up and use it for further queries.
    /// </param>
    public MysqlDataFactory(DataFactoryOptions options, MySqlConnection connection, MySqlTransaction? transaction) 
        : base(options, connection, transaction) 
    {}

    public override IDatabase CreateDatabase()
    {
        _database = new MysqlDatabase(Options.DatabaseOptions, (MySqlConnection) Connection, (MySqlTransaction?) Transaction);
        return _database;
    }

    public override IDataSet<TEntity> CreateDataSet<TEntity>() where TEntity : class
    {
        if (_database == null) throw new MissingDatabaseException();
        
        return new MysqlDataSet<TEntity>(_database, Options.DataSetOptions);
    }

    private MysqlDatabase? _database;
}
