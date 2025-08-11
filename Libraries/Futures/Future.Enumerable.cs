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

public static class IEnumerableExtensions
{
    public static Future<T> ToFuture<T>(this IEnumerable<T> enumerable)
    {
        var future = new Future<IEnumerable<T>, T>((items, producer) =>
        {
            foreach (var value in items)
            {
                producer.Next(value);
            }

            producer.Complete();
        });

        future.NextAsync(enumerable);
        return future;
    }
}

public static class IAsyncEnumerableExtensions
{
    public static Future<T> ToFuture<T>(this IAsyncEnumerable<T> enumerable)
    {
        var future = new Future<IAsyncEnumerable<T>, T>(async (items, producer) =>
        {
            await foreach (var value in items)
            {
                producer.Next(value);
            }

            producer.Complete();
        });

        future.NextAsync(enumerable);
        return future;
    }
}