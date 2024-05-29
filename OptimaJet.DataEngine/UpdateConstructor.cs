using System.Linq.Expressions;
using OptimaJet.DataEngine.Exceptions;
using OptimaJet.DataEngine.Helpers;
using OptimaJet.DataEngine.Queries;
using OptimaJet.DataEngine.Queries.FilterBuilder;
using OptimaJet.DataEngine.Queries.Filters;
using OptimaJet.DataEngine.Queries.Selector;

namespace OptimaJet.DataEngine;

/// <summary>
/// The update query constructor allows you to create queries by chain of calls.
/// The constructor is independent and after creation can be used without a dataset.
/// </summary>
/// <typeparam name="TEntity">A DTO class type that is used to create query elements by expressions.</typeparam>
public class UpdateConstructor<TEntity> where TEntity : class
{
    /// <param name="collection">Dataset object for subsequent query retrieval.</param>
    public UpdateConstructor(ICollection<TEntity> collection)
    {
        _collection = collection;
        _fieldSelector = new FieldSelector();
        _builder = new FilterBuilder();
    }
    
    /// <summary>
    /// Abstract syntax filter tree, used to filter query results with database provider.
    /// </summary>
    public IFilter? Filter { get; private set; }
    
    /// <summary>
    /// Abstract syntax update setter tree, used to update values in the data set.
    /// </summary>
    public Setter? Setter { get; private set; }
    
    /// <summary>
    /// Adds a query filter to filters,
    /// groups all previous filters,
    /// and connects them to this one via logical AND.
    /// </summary>
    /// <param name="fFilter">
    /// Predicate expression that declaratively describes the selection filter.
    /// Used as an expression tree and parsed to create a query.
    /// Unknown constructs will be reduced.
    /// </param>
    /// <returns>This object for crate a chain of calls</returns>
    public UpdateConstructor<TEntity> Where(Expression<Predicate<TEntity>> fFilter)
    {
        Filter = Filter == null 
            ? _builder.Build(fFilter) 
            : new AndFilter(Filter, _builder.Build(fFilter));
        
        return this;
    }
    
    /// <summary>
    /// Adds a query filter to filters,
    /// groups all previous filters,
    /// and connects them to this one via logical AND.
    /// </summary>
    /// <param name="filter">
    /// IFilter object describes the selection filter.
    /// </param>
    /// <returns>This object for crate a chain of calls</returns>
    public UpdateConstructor<TEntity> Where(IFilter filter)
    {
        Filter = Filter == null 
            ? filter
            : new AndFilter(Filter, filter);

        return this;
    }
    
    /// <summary>
    /// Adds a query filter to filters,
    /// groups all previous filters,
    /// and connects them to this one via logical AND.
    /// </summary>
    /// <param name="fFilter">
    /// Predicate expression that declaratively describes the selection filter.
    /// Used as an expression tree and parsed to create a query.
    /// Unknown constructs will be reduced.
    /// </param>
    /// <returns>This object for crate a chain of calls</returns>
    public UpdateConstructor<TEntity> AndWhere(Expression<Predicate<TEntity>> fFilter)
    {
        return Where(fFilter);
    }
    
    /// <summary>
    /// Adds a query filter to filters,
    /// groups all previous filters,
    /// and connects them to this one via logical OR.
    /// </summary>
    /// <param name="fFilter">
    /// Predicate expression that declaratively describes the selection filter.
    /// Used as an expression tree and parsed to create a query.
    /// Unknown constructs will be reduced.
    /// </param>
    /// <returns>This object for crate a chain of calls</returns>
    public UpdateConstructor<TEntity> OrWhere(Expression<Predicate<TEntity>> fFilter)
    {
        Filter = Filter == null 
            ? _builder.Build(fFilter) 
            : new OrFilter(Filter, _builder.Build(fFilter));
        
        return this;
    }
    
    /// <summary>
    /// Adds a query filter to filters,
    /// groups all previous filters,
    /// and connects them to this one via logical Or.
    /// </summary>
    /// <param name="filter">
    /// IFilter object describes the selection filter.
    /// </param>
    /// <returns>This object for crate a chain of calls</returns>
    public UpdateConstructor<TEntity> OrWhere(IFilter filter)
    {
        Filter = Filter == null 
            ? filter
            : new OrFilter(Filter, filter);

        return this;
    }
    
    /// <summary>
    /// Adds an update query set to sets list.
    /// Sets the value for the entities affected by the filters.
    /// If multiple values are specified for one field, the last one will be used.
    /// </summary>
    /// <param name="fFieldSelector">The expression for selecting the set field must return the field of an entity</param>
    /// <param name="value">Value to set the selected field</param>
    /// <typeparam name="TField">Type of the field of an entity for setting value</typeparam>
    /// <returns>This object for crate a chain of calls</returns>
    public UpdateConstructor<TEntity> Set<TField>(Expression<Func<TEntity, TField>> fFieldSelector, TField value)
    {
        Setter ??= new Setter();

        Setter.Fields[_fieldSelector.GetFieldName(fFieldSelector)] = value;

        return this;
    }

    /// <summary>
    /// Adds an update query set to sets list.
    /// Sets the value for the entities affected by the filters.
    /// If multiple values are specified for one field, the last one will be used.
    /// </summary>
    /// <param name="setter">Object that represent a set fields</param>
    /// <returns>This object for crate a chain of calls</returns>
    public UpdateConstructor<TEntity> Set(Setter setter)
    {
        foreach (var (column, value) in setter.Fields)
        {
            Set(column, value);
        }

        return this;
    }

    /// <summary>
    /// Adds an update query set to sets list.
    /// Sets the value for the entities affected by the filters.
    /// If multiple values are specified for one field, the last one will be used.
    /// </summary>
    /// <param name="name">Property name for set</param>
    /// <param name="value">Value for set</param>
    /// <returns>This object for crate a chain of calls</returns>
    public UpdateConstructor<TEntity> Set(string name, object? value)
    {
        var column = _collection.Metadata.Columns.FirstOrDefault(c => c.OriginalName == name);
        if (column == null) throw new MissingColumnException(name);
        
        Setter ??= new Setter();

        Setter.Fields[name] = value;
        
        return this;
    }

    /// <summary>
    /// Executes a composed query
    /// </summary>
    /// <returns>Rowcount that represent successfully deletes</returns>
    public async Task<int> ExecuteAsync()
    {
        Filter = Filter?.Reduce();
        
        switch (Filter)
        {
            case FalseFilter:
                return 0;
            case TrueFilter:
                Filter = null;
                return await _collection.UpdateAsync(this);
            default:
                return await _collection.UpdateAsync(this);
        }
    }

    private readonly ICollection<TEntity> _collection;
    private readonly IFilterBuilder _builder;
    private readonly IFieldSelector _fieldSelector;
}