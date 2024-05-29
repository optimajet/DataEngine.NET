using OptimaJet.DataEngine.Metadata;
using OptimaJet.DataEngine.Queries;
using OptimaJet.DataEngine.Queries.FilterBuilder;
using OptimaJet.DataEngine.Queries.Filters;

namespace OptimaJet.DataEngine.Sql.Queries;

internal sealed class SqlFilterBuilder : FilterVisitor, ISqlFilterBuilder
{
    private SqlFilterBuilder(EntityMetadata metadata)
    {
        Queries = new Stack<SqlKata.Query>();
        OrFlags = new Stack<bool>();
        Metadata = metadata;
    }

    public static ISqlFilterBuilder Create(EntityMetadata metadata)
    {
        return new SqlFilterBuilder(metadata);
    }

    public SqlKata.Query Build(IFilter filter)
    {
        Queries.Push(new SqlKata.Query());
        OrFlags.Push(false);

        filter.Accept(this);

        return Queries.Pop();
    }

    public override IFilter Visit(AndFilter filter)
    {
        Queries.Push(new SqlKata.Query());
        OrFlags.Push(false);

        var modified = base.Visit(filter);

        var subQuery = Queries.Pop();

        OrFlags.Pop();
        if (UseOr) Query.Or();

        Query.Where(_ => subQuery);

        return modified;
    }

    public override IFilter Visit(OrFilter filter)
    {
        Queries.Push(new SqlKata.Query());
        OrFlags.Push(true);

        var modified = base.Visit(filter);

        var subQuery = Queries.Pop();

        OrFlags.Pop();
        if (UseOr) Query.Or();

        Query.Where(_ => subQuery);

        return modified;
    }

    public override IFilter Visit(NotFilter filter)
    {
        Queries.Push(new SqlKata.Query());

        var modified = base.Visit(filter);

        var subQuery = Queries.Pop();

        Query.WhereNot(_ => subQuery);

        return modified;
    }

    public override IFilter Visit(EqualFilter filter)
    {
        AddWhere(filter, "=");

        return filter;
    }

    public override IFilter Visit(NotEqualFilter filter)
    {
        AddWhere(filter, "<>");

        return filter;
    }

    public override IFilter Visit(GreaterFilter filter)
    {
        AddWhere(filter, ">");

        return filter;
    }

    public override IFilter Visit(GreaterEqualFilter filter)
    {
        AddWhere(filter, ">=");

        return filter;
    }

    public override IFilter Visit(LessFilter filter)
    {
        AddWhere(filter, "<");

        return filter;
    }

    public override IFilter Visit(LessEqualFilter filter)
    {
        AddWhere(filter, "<=");

        return filter;
    }

    public override IFilter Visit(IsNullFilter filter)
    {
        AddCondition(q => q.WhereNull(GetTransformedColumnName(filter.Property)));

        return filter;
    }

    public override IFilter Visit(IsNotNullFilter filter)
    {
        AddCondition(q => q.WhereNotNull(GetTransformedColumnName(filter.Property)));

        return filter;
    }

    public override IFilter Visit(IsTrueFilter filter)
    {
        AddCondition(q => q.WhereTrue(GetTransformedColumnName(filter.Property)));

        return filter;
    }

    public override IFilter Visit(IsFalseFilter filter)
    {
        AddCondition(q => q.WhereFalse(GetTransformedColumnName(filter.Property)));

        return filter;
    }

    public override IFilter Visit(LikeFilter filter)
    {
        AddCondition(q => q.WhereLike(GetTransformedColumnName(filter.Property), filter.LikePattern.Value));

        return filter;
    }

    public override IFilter Visit(InFilter filter)
    {
        AddCondition(q => q.WhereIn(GetTransformedColumnName(filter.Property), filter.Array.Values));

        return filter;
    }

    /// <summary>
    /// Current query
    /// </summary>
    private SqlKata.Query Query => Queries.Peek();

    private Stack<SqlKata.Query> Queries { get; }
    private bool UseOr => OrFlags.Peek();
    private Stack<bool> OrFlags { get; }
    private EntityMetadata Metadata { get; }

    private void AddWhere(PropertyConstantFilter filter, string operation)
    {
        AddCondition(q => q.Where(GetTransformedColumnName(filter.Property), operation, filter.Constant.Value));
    }

    private void AddCondition(Func<SqlKata.Query, SqlKata.Query> fAddQueryComponent)
    {
        if (UseOr)
        {
            Query.Or();
        }

        fAddQueryComponent(Query);
    }

    private string GetTransformedColumnName(PropertyFilter filter)
    {
        return Metadata.GetColumnNameByProperty(filter.Name);
    }
}