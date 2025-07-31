namespace Futures.Extensions;

public static partial class IEnumerableExtensions
{
    public static IFuture<TIn, TOut> Pipe<TIn, TOut, TItem>(this TIn _, Func<TIn, TOut> next) where TIn : IEnumerable<TItem> where TOut : IEnumerable<TItem>
    {
        return new Future<TIn, TOut>(value =>
        {
            return next(value);
        });
    }

    public static IFuture<IEnumerable<TItem>, IEnumerable<TItem>> Pipe<TItem>(this IList<TItem> _, Func<IEnumerable<TItem>, IEnumerable<TItem>> next)
    {
        return new Future<IEnumerable<TItem>, IEnumerable<TItem>>(value =>
        {
            return next(value);
        });
    }

    public static IFuture<IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>> Pipe<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> _, Func<IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>> next)
    {
        return new Future<IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>>(value =>
        {
            return next(value);
        });
    }
}