namespace OptimaJet.DataEngine.Sorts;

public class Order
{
    public Order(string originalName, Direction? direction = null)
    {
        OriginalName = originalName;
        Direction = direction ?? Direction.Asc;
    }

    public string OriginalName { get; set; }
    public Direction Direction { get; set; }
}