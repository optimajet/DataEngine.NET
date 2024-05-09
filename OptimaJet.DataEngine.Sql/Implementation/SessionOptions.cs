using System.Data.Common;

namespace OptimaJet.DataEngine.Sql.Implementation;

internal class SessionOptions
{
    public SessionOptions(string connectionString)
    {
        ConnectionString = connectionString;
        ExternalConnection = null;
    }
    
    public SessionOptions(DbConnection externalConnection)
    {
        ExternalConnection = externalConnection;
        ConnectionString = ExternalConnection.ConnectionString;
    }
    
    public DbConnection? ExternalConnection { get; }
    public string ConnectionString { get; }
}