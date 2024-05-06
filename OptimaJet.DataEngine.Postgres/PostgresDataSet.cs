using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Postgres;

public class PostgresDataSet<TEntity> : SqlDataSet<TEntity> where TEntity : class
{
    public PostgresDataSet(PostgresDatabase database, DataSetOptions options) : base(database, options)
    {
        Metadata.SchemaName = options.DatasetSchemaName ?? "public";
    }
}
