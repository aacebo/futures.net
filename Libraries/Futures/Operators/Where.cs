namespace Futures.Operators;

public static partial class FutureExtensions
{
    public static IFuture<IEnumerable<T>> Where<T>(this IFuture<IEnumerable<T>> future, Func<T, bool> predicate)
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static IFuture<IEnumerable<T>> Where<T>(this IFuture<ICollection<T>> future, Func<T, bool> predicate)
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static IFuture<IEnumerable<T>> Where<T>(this IFuture<IList<T>> future, Func<T, bool> predicate)
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static IFuture<IEnumerable<T>> Where<T>(this IFuture<List<T>> future, Func<T, bool> predicate)
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static IFuture<IEnumerable<KeyValuePair<TKey, TValue>>> Where<TKey, TValue>(this IFuture<IDictionary<TKey, TValue>> future, Func<KeyValuePair<TKey, TValue>, bool> predicate) where TKey : notnull
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static IFuture<IEnumerable<KeyValuePair<TKey, TValue>>> Where<TKey, TValue>(this IFuture<Dictionary<TKey, TValue>> future, Func<KeyValuePair<TKey, TValue>, bool> predicate) where TKey : notnull
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static IFuture<T> Where<T>(this ITopic<T> future, Func<T, bool> predicate)
    {
        return new Future<T>((value, producer) =>
        {
            future.Next(value);

            if (predicate(value))
            {
                producer.Next(value);
                producer.Complete();
                return;
            }

            producer.Complete();
        }, future.Token);
    }
}

public static partial class FutureExtensions
{
    public static IStream<T, IEnumerable<TOut>> Where<T, TOut>(this IStream<T, IEnumerable<TOut>> future, Func<TOut, bool> predicate)
    {
        return future.Map(value => value.Where(predicate));
    }

    public static IStream<T, IEnumerable<TOut>> Where<T, TOut>(this IStream<T, ICollection<TOut>> future, Func<TOut, bool> predicate)
    {
        return future.Map(value => value.Where(predicate));
    }

    public static IStream<T, IEnumerable<TOut>> Where<T, TOut>(this IStream<T, IList<TOut>> future, Func<TOut, bool> predicate)
    {
        return future.Map(value => value.Where(predicate));
    }

    public static IStream<T, IEnumerable<TOut>> Where<T, TOut>(this IStream<T, List<TOut>> future, Func<TOut, bool> predicate)
    {
        return future.Map(value => value.Where(predicate));
    }

    public static IStream<T, IEnumerable<KeyValuePair<TKey, TValue>>> Where<T, TKey, TValue>(this IStream<T, IDictionary<TKey, TValue>> future, Func<KeyValuePair<TKey, TValue>, bool> predicate) where TKey : notnull
    {
        return future.Map(value => value.Where(predicate));
    }

    public static IStream<T, IEnumerable<KeyValuePair<TKey, TValue>>> Where<T, TKey, TValue>(this IStream<T, Dictionary<TKey, TValue>> future, Func<KeyValuePair<TKey, TValue>, bool> predicate) where TKey : notnull
    {
        return future.Map(value => value.Where(predicate));
    }

    public static IStream<T, TOut> Where<T, TOut>(this IStream<T, TOut> future, Func<TOut, bool> predicate)
    {
        return new Future<T, TOut>((value, producer) =>
        {
            var output = future.Next(value);

            if (predicate(output))
            {
                producer.Next(output);
                producer.Complete();
                return;
            }

            producer.Complete();
        }, future.Token);
    }
}

public static partial class FutureExtensions
{
    public static IStream<T1, T2, IEnumerable<TOut>> Where<T1, T2, TOut>(this IStream<T1, T2, IEnumerable<TOut>> future, Func<TOut, bool> predicate)
    {
        return future.Map(value => value.Where(predicate));
    }

    public static IStream<T1, T2, IEnumerable<TOut>> Where<T1, T2, TOut>(this IStream<T1, T2, ICollection<TOut>> future, Func<TOut, bool> predicate)
    {
        return future.Map(value => value.Where(predicate));
    }

    public static IStream<T1, T2, IEnumerable<TOut>> Where<T1, T2, TOut>(this IStream<T1, T2, IList<TOut>> future, Func<TOut, bool> predicate)
    {
        return future.Map(value => value.Where(predicate));
    }

    public static IStream<T1, T2, IEnumerable<TOut>> Where<T1, T2, TOut>(this IStream<T1, T2, List<TOut>> future, Func<TOut, bool> predicate)
    {
        return future.Map(value => value.Where(predicate));
    }

    public static IStream<T1, T2, IEnumerable<KeyValuePair<TKey, TValue>>> Where<T1, T2, TKey, TValue>(this IStream<T1, T2, IDictionary<TKey, TValue>> future, Func<KeyValuePair<TKey, TValue>, bool> predicate) where TKey : notnull
    {
        return future.Map(value => value.Where(predicate));
    }

    public static IStream<T1, T2, IEnumerable<KeyValuePair<TKey, TValue>>> Where<T1, T2, TKey, TValue>(this IStream<T1, T2, Dictionary<TKey, TValue>> future, Func<KeyValuePair<TKey, TValue>, bool> predicate) where TKey : notnull
    {
        return future.Map(value => value.Where(predicate));
    }

    public static IStream<T1, T2, TOut> Where<T1, T2, TOut>(this IStream<T1, T2, TOut> future, Func<TOut, bool> predicate)
    {
        return new Future<T1, T2, TOut>((a, b, producer) =>
        {
            var output = future.Next(a, b);

            if (predicate(output))
            {
                producer.Next(output);
                producer.Complete();
                return;
            }

            producer.Complete();
        }, future.Token);
    }
}