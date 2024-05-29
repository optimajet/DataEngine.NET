using System.Data.SQLite;
using OptimaJet.DataEngine.Sql.Implementation;

namespace OptimaJet.DataEngine.Sqlite;

public static class Extensions
{
    public static Task<ITransaction> AttachTransactionAsync(this ISession session, SQLiteTransaction transaction, bool disposeTransaction = false)
    {
        if (session.Provider.Name != ProviderName.Sqlite)
        {
            throw new NotSupportedException("This method is only supported for Sqlite sessions.");
        }

        return ((SqlSession)session).AttachTransactionAsync(transaction, disposeTransaction);
    }
}