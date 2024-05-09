﻿using System.Data.Common;
using Microsoft.Data.SqlClient;
using OptimaJet.DataEngine.Metadata;
using OptimaJet.DataEngine.Mssql.TypeHandlers;
using OptimaJet.DataEngine.Sql;
using OptimaJet.DataEngine.Sql.Implementation;
using OptimaJet.DataEngine.Sql.TypeHandlers;
using SqlKata.Compilers;

namespace OptimaJet.DataEngine.Mssql.Implementation;

internal class MssqlImplementation : ISqlImplementation
{
    static MssqlImplementation()
    {
        Dictionary<Type, ISqlTypeHandler> typeHandlers = new()
        {
            {typeof(TimeSpan), new MssqlTimeSpanHandler()},
            {typeof(TimeSpan?), new MssqlTimeSpanHandler()},
        };

        TypeHandlersPool.SetTypeHandlers(ProviderName.Mssql, typeHandlers);
    }

    public string Name => ProviderName.Mssql;
    public Compiler Compiler { get; } = new SqlServerCompiler();
    public Dialect Dialect { get; } = new MssqlDialect();

    public DbConnection CreateConnection(string connectionString)
    {
        return new SqlConnection(connectionString);
    }

    public void ConfigureMetadata(EntityMetadata metadata)
    {
        // Do nothing
    }
}