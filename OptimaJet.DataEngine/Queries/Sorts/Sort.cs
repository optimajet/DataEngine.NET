namespace OptimaJet.DataEngine.Sorts;

public class Sort
{
    public Sort()
    {
        Orders = new List<Order>();
    }

    public List<Order> Orders { get; }
}