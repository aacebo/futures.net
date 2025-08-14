using System.Collections;

namespace Futures;

public partial class Future<T, TOut> : IEnumerable<TOut>, IAsyncEnumerable<TOut>
{
    public virtual IEnumerator<TOut> GetEnumerator()
    {
        return new Enumerator<T, TOut>(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public virtual IAsyncEnumerator<TOut> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new Enumerator<T, TOut>(this);
    }
}