namespace Futures.Extensions;

public static partial class IEnumerableExtensions
{
    public static Future<TOut> Pipe<T, TOut, TItem>(this T value, Func<T, TOut> next) where T : IEnumerable<TItem> where TOut : IEnumerable<TItem>
    {
        var future = new Future<T, TOut>(next);
        future.Next(value);
        return future;
    }

    public static Future<IEnumerable<TItem>> Pipe<TItem>(this IList<TItem> value, Func<IList<TItem>, IEnumerable<TItem>> next)
    {
        var future = new Future<IList<TItem>, IEnumerable<TItem>>(next);
        future.Next(value);
        return future;
    }

    public static Future<IEnumerable<KeyValuePair<TKey, TValue>>> Pipe<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> value, Func<IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>> next)
    {
        var future = new Future<IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>>(next);
        future.Next(value);
        return future;
    }
}