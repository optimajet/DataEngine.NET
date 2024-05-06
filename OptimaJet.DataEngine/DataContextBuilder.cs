using OptimaJet.DataEngine.Exceptions;

namespace OptimaJet.DataEngine;

/// <summary>
/// Builder for creating identical configured data contexts.
/// </summary>
/// <typeparam name="TContext">Type of building data context</typeparam>
public class DataContextBuilder<TContext> where TContext : DataContext
{
    public DataContextBuilder(Func<IConfigurableDataFactory> getFactoryFn, DataContextOptions? dataContextOptions = null)
    {
        GetFactoryFn = getFactoryFn;
        DataContextOptions = dataContextOptions ?? new DataContextOptions();
    }
    
    /// <summary>
    /// A function returns configurable abstract factory that is placed in the data context when it is created.
    /// </summary>
    public Func<IConfigurableDataFactory> GetFactoryFn { get; set; }
    
    /// <summary>
    /// An options for creating data context.
    /// </summary>
    public DataContextOptions DataContextOptions { get; set; }
    
    /// <summary>
    /// Builds Data context object.
    /// </summary>
    /// <returns>Data context object</returns>
    public TContext Build()
    {
        var factory = GetFactoryFn();
        var dataContext = Activator.CreateInstance(typeof(TContext), factory, DataContextOptions);
        return dataContext as TContext ?? throw new DataContextCreationException();
    }
}