namespace OptimaJet.DataEngine.Sql;

public static class Extensions
{
    public static SqlProvider AsSql(this IProvider provider)
    {
        return (SqlProvider) provider;
    }
}