using OptimaJet.DataEngine.Sql.TypeHandlers;

namespace OptimaJet.DataEngine.Sql;

public class SqlDatabase : Database
{
    public SqlDatabase()
    {
        //TODO this will be deleted in DE-109
        TypeHandlersPool.SetCurrentProvider(Provider.Name);
    }
}