using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using OptimaJet.DataEngine.Exceptions;
using OptimaJet.DataEngine.Filters;
using OptimaJet.DataEngine.Metadata;

namespace OptimaJet.DataEngine.Mongo;

public class MongoFilterBuilder<TEntity> : FilterVisitor, IMongoFilterBuilder<TEntity> where TEntity : class
{
    private MongoFilterBuilder(EntityMetadata metadata)
    {
        Metadata = metadata;
        SubFilters = new Stack<List<FilterDefinition<TEntity>>>();
    }

    public static IMongoFilterBuilder<TEntity> Create(EntityMetadata metadata)
    {
        return new MongoFilterBuilder<TEntity>(metadata);
    }

    public FilterDefinition<TEntity> Build(IFilter filter)
    {
        SubFiltersPushEmpty();
        
        filter.Accept(this);

        var subFilters = SubFilters.Pop();
        
        if (subFilters.Count > 1) throw new FilterCreationException();
        
        return subFilters.Count == 1 
            ? subFilters[0] 
            : Builder.Not(Builder.Empty);
    }

    public override IFilter Visit(AndFilter filter)
    {
        SubFiltersPushEmpty();
        
        var modified = base.Visit(filter);

        var subFilters = SubFilters.Pop();

        if (subFilters.Count != 2) throw new FilterCreationException();

        var mongoFilter = Builder.And(subFilters[0], subFilters[1]);
        
        SubFilters.Peek().Add(mongoFilter);

        return modified;
    }

    public override IFilter Visit(OrFilter filter)
    {
        SubFiltersPushEmpty();
        
        var modified = base.Visit(filter);
        
        var subFilters = SubFilters.Pop();

        if (subFilters.Count != 2) throw new FilterCreationException();
        
        var mongoFilter = Builder.Or(subFilters[0], subFilters[1]);
        
        SubFilters.Peek().Add(mongoFilter);

        return modified;
    }

    public override IFilter Visit(NotFilter filter)
    {
        SubFiltersPushEmpty();
        
        var modified = base.Visit(filter);
        
        var subFilters = SubFilters.Pop();

        if (subFilters.Count != 1) throw new FilterCreationException();
        
        var mongoFilter = Builder.Not(subFilters[0]);
        
        SubFilters.Peek().Add(mongoFilter);

        return modified;
    }

    public override IFilter Visit(EqualFilter filter)
    {
        var subFilter = Builder.Eq(GetTransformedColumnName(filter.Property), filter.Constant.Value);
        SubFilters.Peek().Add(subFilter);
        return filter;
    }

    public override IFilter Visit(NotEqualFilter filter)
    {
        var subFilter = Builder.Not(Builder.Eq(GetTransformedColumnName(filter.Property), filter.Constant.Value));
        SubFilters.Peek().Add(subFilter);
        return filter;
    }

    public override IFilter Visit(GreaterFilter filter)
    {
        var subFilter = Builder.Gt(GetTransformedColumnName(filter.Property), filter.Constant.Value);
        SubFilters.Peek().Add(subFilter);
        return filter;
    }

    public override IFilter Visit(GreaterEqualFilter filter)
    {
        var subFilter = Builder.Gte(GetTransformedColumnName(filter.Property), filter.Constant.Value);
        SubFilters.Peek().Add(subFilter);
        return filter;
    }

    public override IFilter Visit(LessFilter filter)
    {
        var subFilter = Builder.Lt(GetTransformedColumnName(filter.Property), filter.Constant.Value);
        SubFilters.Peek().Add(subFilter);
        return filter;
    }

    public override IFilter Visit(LessEqualFilter filter)
    {
        var subFilter = Builder.Lte(GetTransformedColumnName(filter.Property), filter.Constant.Value);
        SubFilters.Peek().Add(subFilter);
        return filter;
    }

    public override IFilter Visit(IsNullFilter filter)
    {
        var subFilter = Builder.Eq<object?>(GetTransformedColumnName(filter.Property), null);
        SubFilters.Peek().Add(subFilter);
        return filter;
    }

    public override IFilter Visit(IsNotNullFilter filter)
    {
        var subFilter = Builder.Not(Builder.Eq<object?>(GetTransformedColumnName(filter.Property), null));
        SubFilters.Peek().Add(subFilter);
        return filter;
    }

    public override IFilter Visit(IsTrueFilter filter)
    {
        var subFilter = Builder.Eq(GetTransformedColumnName(filter.Property), true);
        SubFilters.Peek().Add(subFilter);
        return filter;
    }

    public override IFilter Visit(IsFalseFilter filter)
    {
        var subFilter = Builder.Eq(GetTransformedColumnName(filter.Property), false);
        SubFilters.Peek().Add(subFilter);
        return filter;
    }

    public override IFilter Visit(LikeFilter filter)
    {
        var like = filter.LikePattern.Constant.Value?.ToString() ?? string.Empty;
        var queryExpr = new BsonRegularExpression(new Regex(Regex.Escape(like), RegexOptions.None));
        var subFilter = Builder.Regex(GetTransformedColumnName(filter.Property), queryExpr);
        SubFilters.Peek().Add(subFilter);
        return filter;
    }

    public override IFilter Visit(InFilter filter)
    {
        var subFilter = Builder.In(GetTransformedColumnName(filter.Property), filter.Array.Values);
        SubFilters.Peek().Add(subFilter);
        return filter;
    }
    
    public override IFilter Visit(TrueFilter filter)
    {
        var subFilter = Builder.Empty;
        SubFilters.Peek().Add(subFilter);
        return filter;
    }
    
    public override IFilter Visit(FalseFilter filter)
    {
        var subFilter = Builder.Not(Builder.Empty);
        SubFilters.Peek().Add(subFilter);
        return filter;
    }

    private EntityMetadata Metadata { get; }
    private FilterDefinition<TEntity>? MongoFilter { get; set; }
    private Stack<List<FilterDefinition<TEntity>>> SubFilters { get; }
    
    private FilterDefinitionBuilder<TEntity> Builder => Builders<TEntity>.Filter;

    private void SubFiltersPushEmpty()
    {
        SubFilters.Push(new List<FilterDefinition<TEntity>>());
    }
    
    private string GetTransformedColumnName(PropertyFilter filter)
    {
        return Metadata.GetColumnNameByProperty(filter.Name);
    }
}