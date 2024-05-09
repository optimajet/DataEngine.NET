using System.Data;
using MissingPrimaryKeyException = OptimaJet.DataEngine.Exceptions.MissingPrimaryKeyException;

namespace OptimaJet.DataEngine.Metadata;

/// <summary>
/// Class that describes the metadata of an entity in a dataset
/// Allows you to get metadata for validation and query generation.
/// It also allows you to work with entity properties to create query results
/// </summary>
public class EntityMetadata
{
    public EntityMetadata(string name)
    {
        OriginalName = name;
        Columns = new List<EntityColumn>();
    }

    public string? SchemaName { get; set; }
    public string OriginalName { get; }
    public string Name => GetNameFn == null ? OriginalName : GetNameFn(OriginalName);
    public Func<string, string>? GetNameFn { get; set; }
    public List<EntityColumn> Columns { get; }
    
    public EntityColumn PrimaryKeyColumn => Columns.FirstOrDefault(c => c.IsPrimaryKey) ?? throw new MissingPrimaryKeyException();
    public IEnumerable<EntityColumn> NotPkColumns => Columns.Where(c => !c.IsPrimaryKey);
    public IEnumerable<string> ColumnNames => Columns.Select(c => c.Name);
    public IEnumerable<string> NotPkColumnNames => NotPkColumns.Select(c => c.Name);
    public IEnumerable<object?> GetColumnValues<TEntity>(TEntity entity) where TEntity : class
    {
        return Columns.Select(c => c.GetValue(entity));
    }

    public DataTable ToDataTable()
    {
        var dt = new DataTable();
        dt.Columns.AddRange(Columns.Select(c => new DataColumn(c.Name, Nullable.GetUnderlyingType(c.RootType) ?? c.RootType)).ToArray());
        return dt;
    }

    public Dictionary<string, object?> ToDictionary<TEntity>(TEntity entity) where TEntity : class
    {
        return Columns.ToDictionary(
            c => c.Name,
            c => c.GetValue(entity)
        );
    }

    public string GetColumnNameByProperty(string name)
    {
        var column = Columns.Find(c => c.OriginalName == name);
        return column.Name;
    }
}