using System.Collections;

namespace OptimaJet.DataEngine;

public class AsyncLocalDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull
{
    private IDictionary<TKey, TValue> Dictionary
    {
        get
        {
            _asyncLocal.Value ??= new Dictionary<TKey, TValue>();
            return _asyncLocal.Value;
        }
    }

    private readonly AsyncLocal<Dictionary<TKey, TValue>> _asyncLocal = new();

    #region IDictionary implementation

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return Dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Dictionary).GetEnumerator();
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Dictionary.Add(item);
    }

    public void Clear()
    {
        Dictionary.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return Dictionary.Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        Dictionary.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Dictionary.Remove(item);
    }

    public int Count => Dictionary.Count;

    public bool IsReadOnly => Dictionary.IsReadOnly;

    public void Add(TKey key, TValue value)
    {
        Dictionary.Add(key, value);
    }

    public bool ContainsKey(TKey key)
    {
        return Dictionary.ContainsKey(key);
    }

    public bool Remove(TKey key)
    {
        return Dictionary.Remove(key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return Dictionary.TryGetValue(key, out value!);
    }

    public TValue this[TKey key]
    {
        get => Dictionary[key];
        set => Dictionary[key] = value;
    }

    public System.Collections.Generic.ICollection<TKey> Keys => Dictionary.Keys;

    public System.Collections.Generic.ICollection<TValue> Values => Dictionary.Values;

    #endregion
}