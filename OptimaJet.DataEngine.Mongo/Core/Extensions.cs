using MongoDB.Driver;

namespace OptimaJet.DataEngine.Mongo;

public static class Extensions
{
    public static Task<ITransaction> AttachSessionAsync(this ISession session, IClientSessionHandle transaction, bool disposeSession = false)
    {
        if (session.Provider.Name != ProviderName.Mongo)
        {
            throw new NotSupportedException("This method is only supported for Mongo sessions.");
        }

        return ((MongoSession)session).AttachSessionAsync(transaction, disposeSession);
    }
}