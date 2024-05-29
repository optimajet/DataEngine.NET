namespace OptimaJet.DataEngine.Mongo;

public class MongoOptions : IOptions
{
    public MongoOptions Clone()
    {
        return new MongoOptions
        {

        };
    }

    object ICloneable.Clone()
    {
        return Clone();
    }
}