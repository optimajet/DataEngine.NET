using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Mssql;

public class MssqlDataSet<TEntity> : SqlDataSet<TEntity> where TEntity : class
{
    public MssqlDataSet(MssqlDatabase database, DataSetOptions options) : base(database, options)
    {
        Metadata.SchemaName = options.DatasetSchemaName ?? "dbo";
    }
}
