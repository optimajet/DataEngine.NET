using System.Data.SQLite;
using OptimaJet.DataEngine.Exceptions;
using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Sqlite;

/// <summary>
/// Factory for implementing Sqlite provider.
/// </summary>
public class SqliteDataFactory : SqlDataFactory
{
    /// <summary>
    /// Creates objects using default options and a new connection.
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    public SqliteDataFactory(string connectionString) 
        : this(new DataFactoryOptions(connectionString), new SQLiteConnection(connectionString), null) 
    {}
    
    /// <summary>
    /// Creates objects using specified options and a new connection.
    /// </summary>
    /// <param name="options">All options that are used to work with data.</param>
    public SqliteDataFactory(DataFactoryOptions options) 
        : this(options, new SQLiteConnection(options.DatabaseOptions.ConnectionString), null) 
    {}
    
    /// <summary>
    /// Creates objects using specified options, connection and transaction.
    /// Do not support nested transactions.
    /// </summary>
    /// <param name="options">All options that are used to work with data.</param>
    /// <param name="connection">Sqlite connection, where you have begun a transaction</param>
    /// <param name="transaction">
    /// Allows you to specify an external transaction
    /// to pick it up and use it for further queries.
    /// </param>
    public SqliteDataFactory(DataFactoryOptions options, SQLiteConnection connection, SQLiteTransaction? transaction) 
        : base(options, connection, transaction) 
    {}

    public override IDatabase CreateDatabase()
    {
        _database = new SqliteDatabase(Options.DatabaseOptions, (SQLiteConnection) Connection, (SQLiteTransaction?) Transaction);
        return _database;
    }

    public override IDataSet<TEntity> CreateDataSet<TEntity>() where TEntity : class
    {
        if (_database == null) throw new MissingDatabaseException();
        
        return new SqliteDataSet<TEntity>(_database, Options.DataSetOptions);
    }

    private SqliteDatabase? _database;
}
