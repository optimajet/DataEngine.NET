using System.Data;

namespace OptimaJet.DataEngine.Sql.TypeHandlers.Default;

internal class Int64Handler : SqlTypeHandler<Int64>
{
    public override void SetValue(IDbDataParameter parameter, long value)
    {
        parameter.DbType = DbType.Int64;
        parameter.Value = value;
    }

    public override long Parse(object value)
    {
        return value switch
        {
            decimal d => Convert.ToInt64(d),
            _ => Convert.ToInt64(value)
        };
    }
}