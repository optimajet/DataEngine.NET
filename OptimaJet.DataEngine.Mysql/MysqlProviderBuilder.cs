using MySql.Data.MySqlClient;
using OptimaJet.DataEngine.Mysql.Implementation;
using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Mysql;

public class MysqlProviderBuilder : SqlProviderBuilder
{
    public MysqlProviderBuilder(string connectionString, bool isUniqueInstance = false)
        : base(new MysqlImplementation(), connectionString, isUniqueInstance)
    {
    }
    
    public MysqlProviderBuilder(MySqlConnection externalConnection, bool isUniqueInstance = false)
        : base(new MysqlImplementation(), externalConnection, isUniqueInstance)
    {
    }
}