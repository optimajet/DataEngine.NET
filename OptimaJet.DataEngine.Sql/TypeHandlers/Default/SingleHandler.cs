using System.Data;

namespace OptimaJet.DataEngine.Sql.TypeHandlers.Default;

public class SingleHandler : SqlTypeHandler<Single>
{
    public override void SetValue(IDbDataParameter parameter, float value)
    {
        parameter.DbType = DbType.Single;
        parameter.Value = value;
    }

    public override float Parse(object value)
    {
        return value switch
        {
            double d => Convert.ToSingle(d),
            _ => Convert.ToSingle(value)
        };
    }
}