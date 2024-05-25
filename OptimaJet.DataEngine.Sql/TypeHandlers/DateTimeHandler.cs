using System.Data;
using Dapper;

namespace OptimaJet.DataEngine.Sql.TypeHandlers.Default;

public class DateTimeHandler : SqlMapper.TypeHandler<DateTime>
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