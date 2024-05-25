using MongoDB.Driver;
using OptimaJet.DataEngine.Mongo.Implementation;

namespace OptimaJet.DataEngine.Mongo;

public static class Extensions
{
    public static MongoProvider AsMongo(this IProvider provider)
    {
        return (MongoProvider) provider;
    }

    public static Task<ITransaction> AttachSessionAsync(this ISession session, IClientSessionHandle transaction, bool disposeSession = false)
    {
        if (session.Provider.Name != ProviderName.Mongo)
        {
            throw new NotSupportedException("This method is only supported for Mongo sessions.");
        }

        return ((MongoSession)session).AttachSessionAsync(transaction, disposeSession);
    }
}