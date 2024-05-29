using System.Data;

namespace OptimaJet.DataEngine.Sql.TypeHandlers;

/// <summary>
/// NOTE. There are only customized types are mentioned here.
/// </summary>
internal static class DefaultTypeMap
{
    private static readonly Dictionary<Type, DbType> Map = new(37)
    {
        {typeof(byte), DbType.Byte},
        {typeof(sbyte), DbType.SByte},
        {typeof(short), DbType.Int16},
        {typeof(ushort), DbType.UInt16},
        {typeof(int), DbType.Int32},
        {typeof(uint), DbType.UInt32},
        {typeof(long), DbType.Int64},
        {typeof(ulong), DbType.UInt64},
        {typeof(float), DbType.Single},
        {typeof(double), DbType.Double},
        {typeof(decimal), DbType.Decimal},
        {typeof(bool), DbType.Boolean},
        {typeof(string), DbType.String},
        {typeof(char), DbType.StringFixedLength},
        {typeof(Guid), DbType.Guid},
        {typeof(DateTime), DbType.DateTime2},
        {typeof(DateTimeOffset), DbType.DateTimeOffset},
        {typeof(TimeSpan), DbType.Time},
        {typeof(byte[]), DbType.Binary},
        {typeof(byte?), DbType.Byte},
        {typeof(sbyte?), DbType.SByte},
        {typeof(short?), DbType.Int16},
        {typeof(ushort?), DbType.UInt16},
        {typeof(int?), DbType.Int32},
        {typeof(uint?), DbType.UInt32},
        {typeof(long?), DbType.Int64},
        {typeof(ulong?), DbType.UInt64},
        {typeof(float?), DbType.Single},
        {typeof(double?), DbType.Double},
        {typeof(decimal?), DbType.Decimal},
        {typeof(bool?), DbType.Boolean},
        {typeof(char?), DbType.StringFixedLength},
        {typeof(Guid?), DbType.Guid},
        {typeof(DateTime?), DbType.DateTime2},
        {typeof(DateTimeOffset?), DbType.DateTimeOffset},
        {typeof(TimeSpan?), DbType.Time},
        {typeof(object), DbType.Object}
    };
    
    public static DbType? Get(Type type)
    {
        return Map.TryGetValue(type, out var dbType) ? dbType : null;
    }
}