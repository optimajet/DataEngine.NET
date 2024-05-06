using System.Data;

namespace OptimaJet.DataEngine.Sql.TypeHandlers.Default;

public class BooleanHandler : SqlTypeHandler<Boolean>
{
    public override void SetValue(IDbDataParameter parameter, bool value)
    {
        parameter.DbType = DbType.Boolean;
        parameter.Value = value;
    }

    public override bool Parse(object value)
    {
        return value switch
        {
            short b => b == 1,
            int i => i == 1,
            long l => l == 1,
            ulong ul => ul == 1,
            string s => s == "1",
            _ => Convert.ToBoolean(value)
        };
    }
}