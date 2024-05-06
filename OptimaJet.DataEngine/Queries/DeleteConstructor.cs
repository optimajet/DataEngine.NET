using System.Linq.Expressions;
using OptimaJet.DataEngine.Filters;

namespace OptimaJet.DataEngine;

/// <summary>
/// The delete query constructor allows you to create queries by chain of calls.
/// The constructor is independent and after creation can be used without a dataset.
/// </summary>
/// <typeparam name="TEntity">A DTO class type that is used to create query elements by expressions.</typeparam>
public class DeleteConstructor<TEntity> where TEntity : class
{
    /// <param name="dataSet">Dataset object for subsequent query retrieval.</param>
    public DeleteConstructor(IDataSet<TEntity> dataSet)
    {
        _dataSet = dataSet;
        _builder = new FilterBuilder();
    }
    
    /// <summary>
    /// Abstract syntax filter tree, used to filter query results with database provider.
    /// </summary>
    public IFilter? Filter { get; private set; }

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
    public DeleteConstructor<TEntity> Where(Expression<Predicate<TEntity>> fFilter)
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
    public DeleteConstructor<TEntity> Where(IFilter filter)
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
    public DeleteConstructor<TEntity> AndWhere(Expression<Predicate<TEntity>> fFilter)
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
    public DeleteConstructor<TEntity> OrWhere(Expression<Predicate<TEntity>> fFilter)
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
    public DeleteConstructor<TEntity> OrWhere(IFilter filter)
    {
        Filter = Filter == null 
            ? filter
            : new OrFilter(Filter, filter);

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
                return await _dataSet.DeleteAsync(this);
            default:
                return await _dataSet.DeleteAsync(this);
        }
    }

    private readonly IDataSet<TEntity> _dataSet;
    private readonly IFilterBuilder _builder;
}