using System.Linq.Expressions;
using OptimaJet.DataEngine.Exceptions;
using OptimaJet.DataEngine.Queries;
using OptimaJet.DataEngine.Queries.FilterBuilder;
using OptimaJet.DataEngine.Queries.Filters;
using OptimaJet.DataEngine.Queries.Selector;

namespace OptimaJet.DataEngine;

/// <summary>
/// The select query constructor allows you to create queries by chain of calls.
/// The constructor is independent and after creation can be used without a dataset.
/// </summary>
/// <typeparam name="TEntity">A DTO class type that is used to create query elements by expressions..</typeparam>
public class SelectConstructor<TEntity> where TEntity : class
{
    /// <param name="collection">Dataset object for subsequent query retrieval.</param>
    public SelectConstructor(ICollection<TEntity> collection)
    {
        _collection = collection;
        _filterBuilder = new FilterBuilder();
        _fieldSelector = new FieldSelector();
    }

    /// <summary>
    /// Abstract syntax filter tree, used to filter query results with database provider.
    /// </summary>
    public IFilter? Filter { get; private set; }
    
    /// <summary>
    /// Abstract syntax sort tree, used to sort query results with database provider.
    /// </summary>
    public Sort? Sort { get; private set; }
    
    /// <summary>
    /// Last specified select limit
    /// </summary>
    public int? Limit { get; private set; }
    
    /// <summary>
    /// Last specified select offset
    /// </summary>
    public int? Offset { get; private set; }
    
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
    public SelectConstructor<TEntity> Where(Expression<Predicate<TEntity>> fFilter)
    {
        return Where(_filterBuilder.Build(fFilter));
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
    public SelectConstructor<TEntity> Where(IFilter filter)
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
    public SelectConstructor<TEntity> AndWhere(Expression<Predicate<TEntity>> fFilter)
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
    public SelectConstructor<TEntity> OrWhere(Expression<Predicate<TEntity>> fFilter)
    {
        return OrWhere(_filterBuilder.Build(fFilter));
    }
    
    /// <summary>
    /// Adds a query filter to filters,
    /// groups all previous filters,
    /// and connects them to this one via logical OR.
    /// </summary>
    /// <param name="filter">
    /// IFilter object describes the selection filter.
    /// </param>
    /// <returns>This object for crate a chain of calls</returns>
    public SelectConstructor<TEntity> OrWhere(IFilter filter)
    {
        Filter = Filter == null 
            ? filter
            : new OrFilter(Filter, filter);

        return this;
    }
    
    /// <summary>
    /// Adds an ascending ordering to sort.
    /// All sorts are performed sequentially in order of addition.
    /// </summary>
    /// <param name="fFieldSelector">The expression for selecting the sort field must return the field of an entity</param>
    /// <returns>This object for crate a chain of calls</returns>
    public SelectConstructor<TEntity> OrderBy<TField>(Expression<Func<TEntity, TField>> fFieldSelector)
    {
        CreateSortIfNotExist();
        
        Sort!.Orders.Add(new Order(_fieldSelector.GetFieldName(fFieldSelector)));
        
        return this;
    }
    
    /// <summary>
    /// Adds an descending ordering to sort.
    /// All sorts are performed sequentially in order of addition.
    /// </summary>
    /// <param name="fFieldSelector">The expression for selecting the sort field must return the field of an entity</param>
    /// <typeparam name="TField">Type of the field of an entity</typeparam>
    /// <returns>This object for crate a chain of calls</returns>
    public SelectConstructor<TEntity> OrderByDesc<TField>(Expression<Func<TEntity, TField>> fFieldSelector)
    {
        CreateSortIfNotExist();
        
        Sort!.Orders.Add(new Order(_fieldSelector.GetFieldName(fFieldSelector), Direction.Desc));
        
        return this;
    }

    /// <summary>
    /// Adds an ordering object to sort.
    /// All sorts are performed sequentially in order of addition.
    /// </summary>
    /// <param name="order">An object that represents the sort order</param>
    /// <returns>This object for crate a chain of calls</returns>
    public SelectConstructor<TEntity> OrderBy(Order order)
    { 
        var column = _collection.Metadata.Columns.FirstOrDefault(c => c.OriginalName == order.OriginalName);
        if (column == null) throw new MissingColumnException(order.OriginalName);

        CreateSortIfNotExist();
        
        Sort!.Orders.Add(order);
        
        return this;
    }

    /// <summary>
    /// Adds an ordering object to sort.
    /// All sorts are performed sequentially in order of addition.
    /// </summary>
    /// <param name="name">Property name for ordering</param>
    /// <param name="direction">Direction of order</param>
    /// <returns>This object for crate a chain of calls</returns>
    public SelectConstructor<TEntity> OrderBy(string name, Direction direction)
    {
        var column = _collection.Metadata.Columns.FirstOrDefault(c => c.OriginalName == name);
        if (column == null) throw new MissingColumnException(name);
        
        OrderBy(new Order(name, direction));
        return this;
    }
    
    /// <summary>
    /// Specifies a select limit for a query
    /// </summary>
    /// <param name="limit">Select limit</param>
    /// <returns>This object for crate a chain of calls</returns>
    public SelectConstructor<TEntity> Take(int limit)
    {
        Limit = limit;
        return this;
    }
    
    /// <summary>
    /// Specifies an offset of selection for a query
    /// </summary>
    /// <param name="offset">Offset of selection</param>
    /// <returns>This object for crate a chain of calls</returns>
    public SelectConstructor<TEntity> Skip(int offset)
    {
        Offset = offset;
        return this;
    }

    /// <summary>
    /// Specifies limit and offset to take specified page
    /// </summary>
    /// <param name="index">Index of taking page, started from zero</param>
    /// <param name="size">Size of taking page</param>
    /// <returns>This object for crate a chain of calls</returns>
    public SelectConstructor<TEntity> Paginate(int index, int size)
    {
        Limit = size;
        Offset = index * size;
        return this;
    }

    /// <summary>
    /// Executes a composed query and returns the result of selection
    /// </summary>
    /// <returns>The result of the selection</returns>
    public async Task<List<TEntity>> GetAsync()
    {
        Filter = Filter?.Reduce();

        switch (Filter)
        {
            case FalseFilter:
                return new List<TEntity>();
            case TrueFilter:
                Filter = null;
                return await _collection.GetAsync(this);
            default:
                return await _collection.GetAsync(this);
        }
    }

    /// <summary>
    /// Executes a composed query and returns the result of selection
    /// </summary>
    /// <returns>The result of the selection</returns>
    public async Task<TEntity?> FirstAsync()
    {
        Filter = Filter?.Reduce();
        
        switch (Filter)
        {
            case FalseFilter:
                return null;
            case TrueFilter:
                Filter = null;
                return await _collection.FirstAsync(this);
            default:
                return await _collection.FirstAsync(this);
        }
    }

    /// <summary>
    /// Executes a composed query and returns the result of counting
    /// </summary>
    /// <returns>The result of the selection</returns>
    public async Task<int> CountAsync()
    {
        Filter = Filter?.Reduce();

        switch (Filter)
        {
            case FalseFilter:
                return 0;
            case TrueFilter:
                Filter = null;
                return await _collection.CountAsync(this);
            default:
                return await _collection.CountAsync(this);
        }
    }
    
    private void CreateSortIfNotExist()
    {
        Sort ??= new Sort();
    }

    private readonly ICollection<TEntity> _collection;
    private readonly IFilterBuilder _filterBuilder;
    private readonly IFieldSelector _fieldSelector;
}