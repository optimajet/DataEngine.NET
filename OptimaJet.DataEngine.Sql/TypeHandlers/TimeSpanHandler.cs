using System.Data;
using Dapper;

namespace OptimaJet.DataEngine.Sql.TypeHandlers;

public class TimeSpanHandler : SqlMapper.TypeHandler<TimeSpan>
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