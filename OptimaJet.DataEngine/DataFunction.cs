// ReSharper disable StaticMemberInGenericType

namespace OptimaJet.DataEngine;

// ReSharper disable once UnusedTypeParameter
public class DataFunction<TEntity, TResult> where TEntity : class
{
    public DataFunction(string name, TEntity? parameter)
    {
        Name = name;
        Parameter = parameter;
    }

    public string Name { get; }
    public TEntity? Parameter { get; }
}