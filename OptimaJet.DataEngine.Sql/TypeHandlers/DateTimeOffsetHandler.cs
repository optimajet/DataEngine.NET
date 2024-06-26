﻿using System.Data;
using Dapper;

namespace OptimaJet.DataEngine.Sql.TypeHandlers;

public class DateTimeOffsetHandler : SqlMapper.TypeHandler<DateTimeOffset>
{
    public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
    {
        parameter.Value = value.ToUniversalTime();
        parameter.DbType = DbType.DateTimeOffset;
    }
    
    public override DateTimeOffset Parse(object value)
    {
        return value switch
        {
            DateTime d => DateTime.SpecifyKind(d, DateTimeKind.Utc),
            long l => DateTime.SpecifyKind(new DateTime(l), DateTimeKind.Utc),
            _ => (DateTimeOffset) value
        };
    }
}