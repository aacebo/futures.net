using System.Collections;

namespace Futures.Collections;

public partial class Stream<T, TOut> : IEnumerable<TOut>, IAsyncEnumerable<TOut>
{
    public IEnumerator<TOut> GetEnumerator()
    {
        return new Enumerator<TOut>(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IAsyncEnumerator<TOut> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new Enumerator<TOut>(this);
    }
}