using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using OptimaJet.DataEngine.Metadata;

namespace OptimaJet.DataEngine;

/// <summary>
/// Interface for managing a single data set using the DTO class.
/// Encapsulates the way data provider data is managed.
/// Allows you to perform CRUD operations on data, various selections through predicative expressions, and so on.
/// The DTO class is used to create metadata that allows the data provider to generate queries.
/// Also, this object is used to transfer the results of selections or to transfer data to the provider.
/// </summary>
/// <typeparam name="TEntity">The type of DTO class that the DataSet will manage.</typeparam>
[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
public interface IDataSet<TEntity> where TEntity : class
{
    /// <summary>
    /// A database object used to generate queries and work with transactions.
    /// </summary>
    IDatabase Database { get; }

    /// <summary>
    /// DTO class metadata
    /// </summary>
    EntityMetadata Metadata { get; }

    #region Select

    /// <summary>
    /// Creates a select query constructor object, allows you to create a query by chain of calls.
    /// </summary>
    /// <returns>Select query constructor object</returns>
    SelectConstructor<TEntity> Select();

    /// <summary>
    /// Used by the select query constructor to execute a composed query.
    /// </summary>
    /// <param name="constructor">Select query constructor object</param>
    /// <returns>The result of the selection</returns>
    Task<List<TEntity>> GetAsync(SelectConstructor<TEntity> constructor);

    /// <summary>
    /// Used by the select query constructor to execute a composed query.
    /// </summary>
    /// <param name="constructor">Select query constructor object</param>
    /// <returns>The first result of the selection</returns>
    Task<TEntity?> FirstAsync(SelectConstructor<TEntity> constructor);
    
    /// <summary>
    /// Used by the select query constructor to execute a composed query with entities counting.
    /// </summary>
    /// <param name="constructor">Select query constructor object</param>
    /// <returns>Count of entities for the query</returns>
    Task<int> CountAsync(SelectConstructor<TEntity> constructor);

    /// <summary>
    /// Selects entities in the data set using a fFilter as the select filter.
    /// </summary>
    /// <param name="fFilter">
    /// Predicate expression that declaratively describes the selection filter.
    /// Used as an expression tree and parsed to create a query.
    /// Unknown constructs will be ignored.
    /// </param>
    /// <returns>Result of selections that satisfy the filter conditions</returns>
    Task<List<TEntity>> GetAsync(Expression<Predicate<TEntity>> fFilter);

    /// <summary>
    /// Selects first entity in the data set using a fFilter as the select filter.
    /// </summary>
    /// <param name="fFilter">
    /// Predicate expression that declaratively describes the selection filter.
    /// Used as an expression tree and parsed to create a query.
    /// Unknown constructs will be ignored.
    /// </param>
    /// <returns>First entity that satisfy the filter conditions</returns>
    Task<TEntity?> FirstAsync(Expression<Predicate<TEntity>> fFilter);
    
    /// <summary>
    /// Count entities in the data set using a fFilter as the select filter.
    /// </summary>
    /// <param name="fFilter">
    /// Predicate expression that declaratively describes the selection filter.
    /// Used as an expression tree and parsed to create a query.
    /// Unknown constructs will be ignored.
    /// </param>
    /// <returns>Count of entities for the query</returns>
    Task<int> CountAsync(Expression<Predicate<TEntity>> fFilter);

    /// <summary>
    /// Selection by the primary key of the data set.
    /// Composite keys are not supported.
    /// If the data set does not have a key, an exception is thrown.
    /// </summary>
    /// <param name="key">The primary key value for the selection.</param>
    /// <returns>The result of selections is an entity or null if none was found.</returns>
    Task<TEntity?> GetByKeyAsync(object key);

    /// <summary>
    /// Selection by enumeration of primary keys.
    /// Composite keys are not supported.
    /// If the data set does not have a key, an exception is thrown.
    /// </summary>
    /// <param name="keys">Primary key values enumeration for the selection.</param>
    /// <returns>Result selected by enumeration of primary keys</returns>
    Task<List<TEntity>> GetByKeysAsync(IEnumerable<object> keys);

    /// <summary>
    /// Selection by array of primary keys.
    /// Composite keys are not supported.
    /// If the data set does not have a key, an exception is thrown.
    /// </summary>
    /// <param name="keys">Primary key values array for the selection.</param>
    /// <returns>Result selected by enumeration of primary keys</returns>
    Task<List<TEntity>> GetByKeysAsync(params object[] keys);

    /// <summary>
    /// Selects all entities in the data set.
    /// </summary>
    /// <returns>The result of selections is all entities in the data set</returns>
    Task<List<TEntity>> GetAsync();
    
    /// <summary>
    /// Selects first entity in the data set.
    /// </summary>
    /// <returns>First entity</returns>
    Task<TEntity?> FirstAsync();
    
    /// <summary>
    /// Count all entities in the data set.
    /// </summary>
    /// <returns>Count of all entities in the data set</returns>
    Task<int> CountAsync();
    
    #endregion

    #region Insert

    /// <summary>
    /// Inserts one entity into the data set.
    /// </summary>
    /// <param name="entity">Entity to insert</param>
    /// <returns>Rowcount that represent successfully inserts</returns>
    Task<int> InsertAsync(TEntity entity);

    /// <summary>
    /// Inserts entities enumeration into the data set.
    /// </summary>
    /// <param name="entities">Entities enumeration to insert</param>
    /// <returns>Rowcount that represent successfully inserts</returns>
    Task<int> InsertAsync(IEnumerable<TEntity> entities);

    #endregion

    #region Update

    /// <summary>
    /// Creates an update query constructor object, allows you to create a query by chain of calls.
    /// </summary>
    /// <returns>Update query constructor object</returns>
    UpdateConstructor<TEntity> Update();

    /// <summary>
    /// Used by the update query constructor to execute a composed query.
    /// </summary>
    /// <param name="constructor">Update query constructor object</param>
    /// <returns>Rowcount that represent successfully updates</returns>
    Task<int> UpdateAsync(UpdateConstructor<TEntity> constructor);

    /// <summary>
    /// Updates entities in the data set using a fFilter as the update filter.
    /// </summary>
    /// <param name="entity">Entity used to replace values in entities affected by the update.</param>
    /// <param name="fFilter">
    /// Predicate expression that declaratively describes the update filter.
    /// Used as an expression tree and parsed to create a query.
    /// Unknown constructs will be ignored.
    /// </param>
    /// <returns>Rowcount that represent successfully updates</returns>
    Task<int> UpdateAsync(TEntity entity, Expression<Predicate<TEntity>> fFilter);

    /// <summary>
    /// Updates an entity in the data set using an entity's primary key value as the update filter.
    /// </summary>
    /// <param name="entity">Entity used to replace values in an entity affected by the update.</param>
    /// <returns>Rowcount that represent successfully updates</returns>
    Task<int> UpdateAsync(TEntity entity);

    #endregion

    #region Upsert

    /// <summary>
    /// Upserts an entity in the data set using an entity's primary key value as the upsert filter.
    /// </summary>
    /// <param name="entity">Entity used to upsert values in an affected entity.</param>
    /// <returns>Rowcount that represent successfully upserts</returns>
    Task<int> UpsertAsync(TEntity entity);

    #endregion

    #region Delete

    /// <summary>
    /// Creates a delete query constructor object, allows you to create a query by chain of calls.
    /// </summary>
    /// <returns>Delete query constructor object</returns>
    DeleteConstructor<TEntity> Delete();

    /// <summary>
    /// Used by the delete query constructor to execute a composed query.
    /// </summary>
    /// <param name="constructor">Delete query constructor object</param>
    /// <returns>Rowcount that represent successfully deletes</returns>
    Task<int> DeleteAsync(DeleteConstructor<TEntity> constructor);

    /// <summary>
    /// Delete entities in the data set using a fFilter as the delete filter.
    /// </summary>
    /// <param name="fFilter">
    /// Predicate expression that declaratively describes the deletion filter.
    /// Used as an expression tree and parsed to create a query.
    /// Unknown constructs will be ignored.
    /// </param>
    /// <returns>Rowcount that represent successfully deletes</returns>
    Task<int> DeleteAsync(Expression<Predicate<TEntity>> fFilter);

    /// <summary>
    /// Delete an entity in the data set using an entity's primary key value as the delete filter.
    /// </summary>
    /// <param name="entity">Entity used to generate a delete filter.</param>
    /// <returns>Rowcount that represent successfully deletes</returns>
    Task<int> DeleteAsync(TEntity entity);

    /// <summary>
    /// Deletes entities from the data set using an entities primary key values as the delete filter.
    /// </summary>
    /// <param name="entities">Entities enumeration used to generate a delete filter.</param>
    /// <returns>Rowcount that represent successfully deletes</returns>
    Task<int> DeleteAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// Deletion by the primary key of the data set.
    /// Composite keys are not supported.
    /// If the data set does not have a key, an exception is thrown.
    /// </summary>
    /// <param name="key">The primary key value for the deletion.</param>
    /// <returns>Rowcount that represent successfully deletes.</returns>
    Task<int> DeleteByKeyAsync(object key);

    /// <summary>
    /// Deletion by enumeration of primary keys.
    /// Composite keys are not supported.
    /// If the data set does not have a key, an exception is thrown.
    /// </summary>
    /// <param name="keys">Primary key values enumeration for the deletion.</param>
    /// <returns>Rowcount that represent successfully deletes.</returns>
    Task<int> DeleteByKeysAsync(IEnumerable<object> keys);
    
    /// <summary>
    /// Deletion by array of primary keys.
    /// Composite keys are not supported.
    /// If the data set does not have a key, an exception is thrown.
    /// </summary>
    /// <param name="keys">Primary key values array for the deletion.</param>
    /// <returns>Rowcount that represent successfully deletes.</returns>
    Task<int> DeleteByKeysAsync(params object[] keys);

    /// <summary>
    /// Deletes all entities in the data set.
    /// </summary>
    /// <returns>Rowcount that represent successfully deletes.</returns>
    Task<int> DeleteAllAsync();

    #endregion
}