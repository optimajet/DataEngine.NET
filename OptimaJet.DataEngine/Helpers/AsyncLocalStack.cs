using System.Collections;

namespace OptimaJet.DataEngine.Helpers;

internal class AsyncLocalStack<T> : ICollection, IReadOnlyCollection<T>
{
    private Stack<T> Stack
    {
        get
        {
            _asyncLocal.Value ??= new Stack<T>();
            return _asyncLocal.Value;
        }
    }

    private readonly AsyncLocal<Stack<T>> _asyncLocal = new();

    #region Stack implementation

    public void Clear()
    {
        Stack.Clear();
    }

    public bool Contains(T item)
    {
        return Stack.Contains(item);
    }

    public T Peek()
    {
        return Stack.Peek();
    }

    public T Pop()
    {
        return Stack.Pop();
    }

    public void Push(T item)
    {
        Stack.Push(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        Stack.CopyTo(array, arrayIndex);
    }

    public T[] ToArray()
    {
        return Stack.ToArray();
    }

    void TrimExcess()
    {
        Stack.TrimExcess();
    }

    public bool TryPeek(out T result)
    {
        if (Stack.Count == 0)
        {
            result = default!;
            return false;
        }

        result = Stack.Peek();
        return true;
    }

    public bool TryPop(out T result)
    {
        if (Stack.Count == 0)
        {
            result = default!;
            return false;
        }

        result = Stack.Pop();
        return true;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return Stack.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Stack).GetEnumerator();
    }

    public int Count => Stack.Count;

    void ICollection.CopyTo(Array array, int index)
    {
        ((ICollection)Stack).CopyTo(array, index);
    }

    public bool IsSynchronized => ((ICollection)Stack).IsSynchronized;

    public object SyncRoot => ((ICollection)Stack).SyncRoot;

    #endregion
}