namespace OptimaJet.DataEngine.Helpers;

internal static class Extensions
{
    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> tuple, out TKey key, out TValue value)
    {
        key = tuple.Key;
        value = tuple.Value;
    }
}