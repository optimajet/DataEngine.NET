namespace OptimaJet.DataEngine;

public class Database
{
    public IProvider Provider => ProviderContext.Current;
    public ISession Session => Provider.Session;
}
