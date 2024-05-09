﻿using System.Data;

namespace OptimaJet.DataEngine.Sql.TypeHandlers.Default;

internal class ByteHandler : SqlTypeHandler<Byte>
{
    public override void SetValue(IDbDataParameter parameter, byte value)
    {
        parameter.DbType = DbType.Byte;
        parameter.Value = value;
    }

    public override byte Parse(object value)
    {
        return value switch
        {
            short s => Convert.ToByte(s & 255),
            long l => Convert.ToByte(l),
            sbyte sb => Convert.ToByte(sb),
            _ => Convert.ToByte(value)
        };
    }
}