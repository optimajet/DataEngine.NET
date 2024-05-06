using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Sqlite;

public class SqliteDataSet<TEntity> : SqlDataSet<TEntity> where TEntity : class
{
    public SqliteDataSet(SqliteDatabase database, DataSetOptions options) : base(database, options)
    {
        Metadata.SchemaName = options.DatasetSchemaName ?? "main";
    }
}
