﻿using System.Data.Common;
using MySqlConnector;
using OptimaJet.DataEngine.Metadata;
using OptimaJet.DataEngine.Mysql.TypeHandlers;
using OptimaJet.DataEngine.Sql;
using OptimaJet.DataEngine.Sql.Implementation;
using OptimaJet.DataEngine.Sql.TypeHandlers;
using SqlKata.Compilers;

namespace OptimaJet.DataEngine.Mysql.Implementation;

internal class MysqlImplementation : ISqlImplementation
{
    static MysqlImplementation()
    {
        TypeHandlerRegistry.RegisterDefault(new MysqlDateTimeHandler(), ProviderName.Mysql);
        TypeHandlerRegistry.RegisterDefault(new MysqlDateTimeOffsetHandler(), ProviderName.Mysql);
    }
    
    public string Name => ProviderName.Mysql;
    public Compiler Compiler { get; } = new MySqlCompiler();
    public Dialect Dialect { get; } = new MysqlDialect();
    
    public DbConnection CreateConnection(string connectionString)
    {
        return new MySqlConnection(connectionString);
    }
    
    public void ConfigureMetadata(EntityMetadata metadata)
    {
        metadata.GetNameFn ??= name => name.ToLowerInvariant();
    }
}