namespace OptimaJet.DataEngine.Filters;

public interface IFilterReducer
{ 
    IFilter Reduce(IFilter filter);
}