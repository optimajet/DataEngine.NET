namespace OptimaJet.DataEngine.Queries.FilterReducer;

internal interface IFilterReducer
{ 
    IFilter Reduce(IFilter filter);
}