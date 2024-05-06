using System.Data;

namespace OptimaJet.DataEngine.Sql.TypeHandlers.Default;

public class GuidHandler : SqlTypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        parameter.DbType = DbType.Guid;
        parameter.Value = value;
    }

    public override Guid Parse(object value)
    {
        return value switch
        {
            string s => Guid.Parse(s),
            byte[] b => new Guid(b),
            _ => (Guid) value
        };
    }
}