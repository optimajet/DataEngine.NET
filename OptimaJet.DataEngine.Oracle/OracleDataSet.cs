using OptimaJet.DataEngine.Metadata;
using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Oracle;

public class OracleDataSet<TEntity> : SqlDataSet<TEntity> where TEntity : class
{
    static OracleDataSet()
    {
        var metadata = MetadataPool<TEntity>.GetMetadata(ProviderType.Oracle);
        
        metadata.GetNameFn ??= name => name.ToUpperInvariant();

        foreach (var column in metadata.Columns)
        {
            column.GetNameFn ??= n => n.ToUpperInvariant();
        }
    }
    
    /// <summary>
    /// Initializes static constructor
    /// </summary>
    public static void Activate() {}
    
    public OracleDataSet(OracleDatabase database, DataSetOptions options) : base(database, options)
    {
        Metadata.SchemaName = options.DatasetSchemaName;
    }
}
