using System.Data.Common;
using OptimaJet.DataEngine.Sql;
using OptimaJet.DataEngine.Sql.Implementation;

namespace OptimaJet.DataEngine.Oracle;

public static class Extensions
{
    public static Task<ITransaction> AttachTransactionAsync(this ISession session, DbTransaction transaction, bool disposeTransaction = false)
    {
        if (session.Provider.Name != ProviderName.Oracle)
        {
            throw new NotSupportedException("This method is only supported for Oracle sessions.");
        }

        return ((SqlSession)session).AttachTransactionAsync(transaction, disposeTransaction);
    }
}