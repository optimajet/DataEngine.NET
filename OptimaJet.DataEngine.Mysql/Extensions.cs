using MySql.Data.MySqlClient;
using OptimaJet.DataEngine.Sql;
using OptimaJet.DataEngine.Sql.Implementation;

namespace OptimaJet.DataEngine.Mysql;

public static class Extensions
{
    public static Task<ITransaction> AttachTransactionAsync(this ISession session, MySqlTransaction transaction, bool disposeTransaction = false)
    {
        if (session.Provider.Name != ProviderName.Mysql)
        {
            throw new NotSupportedException("This method is only supported for Mysql sessions.");
        }

        return ((SqlSession)session).AttachTransactionAsync(transaction, disposeTransaction);
    }
}