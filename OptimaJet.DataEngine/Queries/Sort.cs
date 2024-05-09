namespace OptimaJet.DataEngine.Queries;

public class Sort
{
    public Sort()
    {
        Orders = new List<Order>();
    }

    public List<Order> Orders { get; }
}