using OptimaJet.DataEngine.Metadata;
using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Mysql;

public class MysqlDataSet<TEntity> : SqlDataSet<TEntity> where TEntity : class
{
    static MysqlDataSet()
    {
        var metadata = MetadataPool<TEntity>.GetMetadata(ProviderType.Mysql);
        
        metadata.GetNameFn ??= name => name.ToLowerInvariant();
    }
    
    /// <summary>
    /// Initializes static constructor
    /// </summary>
    public static void Activate() {}

    public MysqlDataSet(MysqlDatabase database, DataSetOptions options) : base(database, options)
    {
        Metadata.SchemaName = null;
    }
}
