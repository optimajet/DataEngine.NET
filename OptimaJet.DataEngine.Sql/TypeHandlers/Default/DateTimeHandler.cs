using System.Data;

namespace OptimaJet.DataEngine.Sql.TypeHandlers.Default;

internal class DateTimeHandler : SqlTypeHandler<DateTime>
{
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        parameter.Value = value;
        parameter.DbType = DbType.DateTime2;
    }

    public override DateTime Parse(object value)
    {
        return value switch
        {
            long l => new DateTime(l),
            _ => Convert.ToDateTime(value)
        };
    }
}