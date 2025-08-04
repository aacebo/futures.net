using System.Collections;

namespace Futures;

public partial class Future<TIn, TOut>
{
    public IEnumerator<TOut> GetEnumerator()
    {
        return new FutureEnumerator<TIn, TOut>(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal partial class FutureEnumerator<TIn, TOut> : IEnumerator<TOut>
{
    object IEnumerator.Current => Current!;

    public void Dispose()
    {
        _subscription.Dispose();
        GC.SuppressFinalize(this);
    }

    public bool MoveNext()
    {
        _index++;

        while (_index >= _values.Count && !_complete)
        {
            Task.Delay(100).Wait();
        }

        return _index < _values.Count;
    }

    public void Reset()
    {
        throw new NotSupportedException();
    }
}