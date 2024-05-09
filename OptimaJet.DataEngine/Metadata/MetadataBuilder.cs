using System.Reflection;
using Humanizer;
using OptimaJet.DataEngine.Attributes;
using OptimaJet.DataEngine.Exceptions;

namespace OptimaJet.DataEngine.Metadata;

internal static class MetadataBuilder
{
    public static EntityMetadata Build<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity);

        var tableNameAttribute = type.GetCustomAttributes().FirstOrDefault(a => a is TableNameAttribute) as TableNameAttribute;
        var typeName = type.Name;
        var tableName = tableNameAttribute?.Name ?? typeName.Pluralize() ?? typeName;
        var metadata = new EntityMetadata(tableName);
        
        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            metadata.Columns.Add(GetColumn(
                property.Name, 
                property.PropertyType, 
                property.GetCustomAttributes().ToList()));
        }
        
        var fields = type.GetFields();

        foreach (var field in fields)
        {
            metadata.Columns.Add(GetColumn(
                field.Name, 
                field.FieldType, 
                field.GetCustomAttributes().ToList()));
        }

        return metadata;
    }

    private static EntityColumn GetColumn(string name, Type type, List<Attribute> attributes)
    {
        var column = new EntityColumn(name, type)
        {
            Type = GetType(type, attributes),
        };

        column.Constraints = GetConstraints(column, attributes);

        return column;
    }

    private static List<ColumnConstraint> GetConstraints(EntityColumn column, List<Attribute> attributes)
    {
        var constraints = new List<ColumnConstraint>();

        foreach (var attribute in attributes)
        {
            var constraint = attribute switch
            {
                PrimaryKeyAttribute => new ColumnConstraint($"{column.Name}PrimaryKey") {Type = ConstraintType.PrimaryKey},
                _ => null
            };

            if (constraint != null) constraints.Add(constraint);
        }

        return constraints;
    }

    private static ColumnType GetType(Type type, List<Attribute> attributes)
    {
        var columnType = new ColumnType();

        if (type.Name == typeof(Nullable<>).Name)
        {
            columnType.Nullable = true;
            type = Nullable.GetUnderlyingType(type) ?? throw new TypeNotSupportedException();
        }
        
        if (type.IsEnum)
        {
            type = type.GetEnumUnderlyingType();
        }

        if (type.IsClass)
        {
            columnType.Nullable = !attributes.Any(a => a is DataRequiredAttribute);
        }

        if (type.IsArray)
        {
            columnType.Enumerable = true;
            type = type.GetElementType() ?? throw new TypeNotSupportedException();
        }
        
        //Length

        if (attributes.FirstOrDefault(a => a is DataLengthAttribute) is DataLengthAttribute dataLengthAttribute)
        {
            columnType.Length = dataLengthAttribute.Length;
        }

        columnType.Type = type.Name switch
        {
            nameof(Byte) => DataType.Byte,
            nameof(Int16) => DataType.Int16,
            nameof(Int32) => DataType.Int32,
            nameof(Int64) => DataType.Int64,
            nameof(Double) => DataType.Double,
            nameof(Decimal) => DataType.Decimal,
            nameof(Single) => DataType.Single,
            nameof(Boolean) => DataType.Boolean,
            nameof(String) => DataType.String,
            nameof(DateTime) => DataType.DateTime,
            nameof(DateTimeOffset) => DataType.DateTimeOffset,
            nameof(TimeSpan) => DataType.TimeSpan,
            nameof(Guid) => DataType.Guid,
            _ => throw new TypeNotSupportedException()
        };

        return columnType;
    }
}