using OptimaJet.DataEngine.Sql.TypeHandlers;

namespace OptimaJet.DataEngine.Sql;

public class SqlDataContext : DataContext
{
    public SqlDataContext(IDataFactory factory, DataContextOptions? options = null) : base(factory, options)
    {
        //TODO this will be deleted in DE-109
        TypeHandlersPool.SetCurrentProvider(Database.ProviderType);
    }
}