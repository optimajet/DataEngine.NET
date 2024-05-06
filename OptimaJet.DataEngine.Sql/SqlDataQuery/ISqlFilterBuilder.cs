using OptimaJet.DataEngine.Filters;
using SqlKata;

namespace OptimaJet.DataEngine.Sql.SqlDataQuery;

public interface ISqlFilterBuilder
{
    Query Build(IFilter filter);
}