using System.Collections;

namespace Futures;

public partial class Future<T, TOut> : IEnumerable<TOut>, IAsyncEnumerable<TOut>
{
    public IEnumerator<TOut> GetEnumerator()
    {
        return new Enumerator<TOut>(Out);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IAsyncEnumerator<TOut> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new Enumerator<TOut>(Out);
    }
}

public partial class Future<T1, T2, TOut> : IEnumerable<TOut>, IAsyncEnumerable<TOut>
{
    public IEnumerator<TOut> GetEnumerator()
    {
        return new Enumerator<TOut>(Out);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IAsyncEnumerator<TOut> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new Enumerator<TOut>(Out);
    }
}