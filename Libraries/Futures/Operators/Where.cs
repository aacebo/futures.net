namespace Futures.Operators;

public static partial class FutureExtensions
{
    public static Future<IEnumerable<T>> Where<T>(this Future<IEnumerable<T>> future, Func<T, bool> predicate)
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static Future<IEnumerable<T>> Where<T>(this Future<ICollection<T>> future, Func<T, bool> predicate)
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static Future<IEnumerable<T>> Where<T>(this Future<IList<T>> future, Func<T, bool> predicate)
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static Future<IEnumerable<T>> Where<T>(this Future<List<T>> future, Func<T, bool> predicate)
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static Future<IEnumerable<KeyValuePair<TKey, TValue>>> Where<TKey, TValue>(this Future<IDictionary<TKey, TValue>> future, Func<KeyValuePair<TKey, TValue>, bool> predicate) where TKey : notnull
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static Future<IEnumerable<KeyValuePair<TKey, TValue>>> Where<TKey, TValue>(this Future<Dictionary<TKey, TValue>> future, Func<KeyValuePair<TKey, TValue>, bool> predicate) where TKey : notnull
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static Future<T> Where<T>(this Future<T> future, Func<T, bool> predicate)
    {
        return new Future<T>((value, producer) =>
        {
            var output = future.Next(value);

            if (predicate(output))
            {
                producer.Next(output);
                producer.Complete();
                return;
            }

            producer.Complete();
        });
    }
}

public static partial class FutureExtensions
{
    public static Future<T, IEnumerable<TOut>> Where<T, TOut>(this Future<T, IEnumerable<TOut>> future, Func<TOut, bool> predicate)
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static Future<T, IEnumerable<TOut>> Where<T, TOut>(this Future<T, ICollection<TOut>> future, Func<TOut, bool> predicate)
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static Future<T, IEnumerable<TOut>> Where<T, TOut>(this Future<T, IList<TOut>> future, Func<TOut, bool> predicate)
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static Future<T, IEnumerable<TOut>> Where<T, TOut>(this Future<T, List<TOut>> future, Func<TOut, bool> predicate)
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static Future<T, IEnumerable<KeyValuePair<TKey, TValue>>> Where<T, TKey, TValue>(this Future<T, IDictionary<TKey, TValue>> future, Func<KeyValuePair<TKey, TValue>, bool> predicate) where TKey : notnull
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static Future<T, IEnumerable<KeyValuePair<TKey, TValue>>> Where<T, TKey, TValue>(this Future<T, Dictionary<TKey, TValue>> future, Func<KeyValuePair<TKey, TValue>, bool> predicate) where TKey : notnull
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static Future<T, TOut> Where<T, TOut>(this Future<T, TOut> future, Func<TOut, bool> predicate)
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
        });
    }
}

public static partial class FutureExtensions
{
    public static Future<T1, T2, IEnumerable<TOut>> Where<T1, T2, TOut>(this Future<T1, T2, IEnumerable<TOut>> future, Func<TOut, bool> predicate)
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static Future<T1, T2, IEnumerable<TOut>> Where<T1, T2, TOut>(this Future<T1, T2, ICollection<TOut>> future, Func<TOut, bool> predicate)
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static Future<T1, T2, IEnumerable<TOut>> Where<T1, T2, TOut>(this Future<T1, T2, IList<TOut>> future, Func<TOut, bool> predicate)
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static Future<T1, T2, IEnumerable<TOut>> Where<T1, T2, TOut>(this Future<T1, T2, List<TOut>> future, Func<TOut, bool> predicate)
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static Future<T1, T2, IEnumerable<KeyValuePair<TKey, TValue>>> Where<T1, T2, TKey, TValue>(this Future<T1, T2, IDictionary<TKey, TValue>> future, Func<KeyValuePair<TKey, TValue>, bool> predicate) where TKey : notnull
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static Future<T1, T2, IEnumerable<KeyValuePair<TKey, TValue>>> Where<T1, T2, TKey, TValue>(this Future<T1, T2, Dictionary<TKey, TValue>> future, Func<KeyValuePair<TKey, TValue>, bool> predicate) where TKey : notnull
    {
        return future.Pipe(value => value.Where(predicate));
    }

    public static Future<T1, T2, TOut> Where<T1, T2, TOut>(this Future<T1, T2, TOut> future, Func<TOut, bool> predicate)
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
        });
    }
}