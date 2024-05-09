using OptimaJet.DataEngine.Queries;

namespace OptimaJet.DataEngine.Sql.Queries;

internal interface ISqlFilterBuilder
{
    SqlKata.Query Build(IFilter filter);
}