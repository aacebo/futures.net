namespace Futures.Operators;

public static partial class OperatorExtensions
{
    public static IFuture<IEnumerable<TItem>, IEnumerable<TItem>> Where<TItem>(this IFuture<IEnumerable<TItem>, IEnumerable<TItem>> future, Func<TItem, bool> predicate)
    {
        return new Future<IEnumerable<TItem>, IEnumerable<TItem>>(value =>
        {
            return future.Next(value).Where(predicate);
        });
    }

    public static IFuture<ICollection<TItem>, IEnumerable<TItem>> Where<TItem>(this IFuture<ICollection<TItem>, ICollection<TItem>> future, Func<TItem, bool> predicate)
    {
        return new Future<ICollection<TItem>, IEnumerable<TItem>>(value =>
        {
            return future.Next(value).Where(predicate);
        });
    }

    public static IFuture<IDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>> Where<TKey, TValue>(this IFuture<IDictionary<TKey, TValue>, IDictionary<TKey, TValue>> future, Func<KeyValuePair<TKey, TValue>, bool> predicate)
    {
        return new Future<IDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>>(value =>
        {
            return future.Next(value).Where(predicate);
        });
    }

    public static IFuture<Dictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>> Where<TKey, TValue>(this IFuture<Dictionary<TKey, TValue>, Dictionary<TKey, TValue>> future, Func<KeyValuePair<TKey, TValue>, bool> predicate) where TKey : notnull
    {
        return new Future<Dictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>>(value =>
        {
            return future.Next(value).Where(predicate);
        });
    }

    public static IFuture<IList<TItem>, IEnumerable<TItem>> Where<TItem>(this IFuture<IList<TItem>, IList<TItem>> future, Func<TItem, bool> predicate)
    {
        return new Future<IList<TItem>, IEnumerable<TItem>>(value =>
        {
            return future.Next(value).Where(predicate);
        });
    }

    public static IFuture<List<TItem>, IEnumerable<TItem>> Where<TItem>(this IFuture<List<TItem>, List<TItem>> future, Func<TItem, bool> predicate)
    {
        return new Future<List<TItem>, IEnumerable<TItem>>(value =>
        {
            return future.Next(value).Where(predicate);
        });
    }

    public static IFuture<TIn, TOut> Where<TIn, TOut>(this IFuture<TIn, TOut> future, Func<TOut, bool> predicate)
    {
        return new Future<TIn, TOut>((value, subscriber) =>
        {
            var output = future.Next(value);

            if (predicate(output))
            {
                subscriber.Next(output);
                subscriber.Complete();
                return;
            }

            subscriber.Complete();
        });
    }
}