using System.Data;

namespace OptimaJet.DataEngine.Sql.TypeHandlers.Default;

public class TimeSpanHandler : SqlTypeHandler<TimeSpan>
{
    public override void SetValue(IDbDataParameter parameter, TimeSpan value)
    {
        parameter.Value = value;
        parameter.DbType = DbType.Time;
    }

    public override TimeSpan Parse(object value)
    {
        return value switch
        {
            long l => new TimeSpan(l),
            _ => (TimeSpan) value
        };
    }
}