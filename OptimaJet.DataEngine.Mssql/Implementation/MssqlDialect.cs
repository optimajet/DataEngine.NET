﻿using OptimaJet.DataEngine.Sql;

namespace OptimaJet.DataEngine.Mssql.Implementation;

internal class MssqlDialect : Dialect
{
    protected override string LeftQuote => "[";
    protected override string RightQuote => "]";
}