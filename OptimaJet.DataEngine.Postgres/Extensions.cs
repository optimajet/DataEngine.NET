using Npgsql;
using OptimaJet.DataEngine.Sql;
using OptimaJet.DataEngine.Sql.Implementation;

namespace OptimaJet.DataEngine.Postgres;

public static class Extensions
{
    public static Task<ITransaction> AttachTransactionAsync(this ISession session, NpgsqlTransaction transaction, bool disposeTransaction = false)
    {
        if (session.Provider.Name != ProviderName.Postgres)
        {
            throw new NotSupportedException("This method is only supported for Postgres sessions.");
        }

        return ((SqlSession)session).AttachTransactionAsync(transaction, disposeTransaction);
    }
}