using MySql.Data.MySqlClient;
using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Mysql;

public class MysqlProviderBuilder : SqlProviderBuilder
{
    public MysqlProviderBuilder(string connectionString, bool isUniqueInstance)
        : base(new MysqlImplementation(), new SessionOptions(connectionString), isUniqueInstance)
    {
    }
    
    public MysqlProviderBuilder(MySqlConnection externalConnection, bool isUniqueInstance)
        : base(new MysqlImplementation(), new SessionOptions(externalConnection), isUniqueInstance)
    {
    }
}