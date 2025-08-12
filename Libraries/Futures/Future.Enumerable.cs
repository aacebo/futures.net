using System.Collections;

namespace Futures;

public partial class Future<T> : IEnumerable<T>, IAsyncEnumerable<T>
{
    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator<T>(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new Enumerator<T>(this);
    }
}

public partial class Future<T, TOut> : IEnumerable<TOut>, IAsyncEnumerable<TOut>
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

public partial class Future<T1, T2, TOut> : IEnumerable<TOut>, IAsyncEnumerable<TOut>
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

public static class IAsyncEnumerableExtensions
{
    public static IFuture<T> ToFuture<T>(this IAsyncEnumerable<T> enumerable)
    {
        var future = new Future<IAsyncEnumerable<T>, T>(async (items, producer) =>
        {
            await foreach (var value in items)
            {
                producer.Next(value);
            }

            producer.Complete();
        });

        future.Next(enumerable);
        return future;
    }
}