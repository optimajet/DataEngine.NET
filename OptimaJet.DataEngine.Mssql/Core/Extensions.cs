using Microsoft.Data.SqlClient;
using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Mssql;

public static class Extensions
{
    public static Task<ITransaction> AttachTransactionAsync(this ISession session, SqlTransaction transaction, bool disposeTransaction = false)
    {
        if (session.Provider.Name != ProviderName.Mssql)
        {
            throw new NotSupportedException("This method is only supported for Mssql sessions.");
        }

        return ((SqlSession)session).AttachTransactionAsync(transaction, disposeTransaction);
    }
}