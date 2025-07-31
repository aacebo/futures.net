namespace Futures.Operators;

public static partial class OperatorExtensions
{
    public static IFuture<IEnumerable<TIn>, IEnumerable<TOut>> Where<TIn, TOut>(this IFuture<IEnumerable<TIn>, IEnumerable<TOut>> future, Func<TOut, bool> predicate)
    {
        return new Future<IEnumerable<TIn>, IEnumerable<TOut>>(value =>
        {
            return future.Next(value).Where(predicate);
        });
    }

    public static IFuture<ICollection<TIn>, IEnumerable<TOut>> Where<TIn, TOut>(this IFuture<ICollection<TIn>, ICollection<TOut>> future, Func<TOut, bool> predicate)
    {
        return new Future<ICollection<TIn>, IEnumerable<TOut>>(value =>
        {
            return future.Next(value).Where(predicate);
        });
    }

    public static IFuture<IDictionary<TKeyIn, TValueIn>, IEnumerable<KeyValuePair<TKeyOut, TValueOut>>> Where<TKeyIn, TValueIn, TKeyOut, TValueOut>(this IFuture<IDictionary<TKeyIn, TValueIn>, IDictionary<TKeyOut, TValueOut>> future, Func<KeyValuePair<TKeyOut, TValueOut>, bool> predicate) where TKeyIn : notnull where TKeyOut : notnull
    {
        return new Future<IDictionary<TKeyIn, TValueIn>, IEnumerable<KeyValuePair<TKeyOut, TValueOut>>>(value =>
        {
            return future.Next(value).Where(predicate);
        });
    }

    public static IFuture<Dictionary<TKeyIn, TValueIn>, IEnumerable<KeyValuePair<TKeyOut, TValueOut>>> Where<TKeyIn, TValueIn, TKeyOut, TValueOut>(this IFuture<Dictionary<TKeyIn, TValueIn>, Dictionary<TKeyOut, TValueOut>> future, Func<KeyValuePair<TKeyOut, TValueOut>, bool> predicate) where TKeyIn : notnull where TKeyOut : notnull
    {
        return new Future<Dictionary<TKeyIn, TValueIn>, IEnumerable<KeyValuePair<TKeyOut, TValueOut>>>(value =>
        {
            return future.Next(value).Where(predicate);
        });
    }

    public static IFuture<IList<TIn>, IEnumerable<TOut>> Where<TIn, TOut>(this IFuture<IList<TIn>, IList<TOut>> future, Func<TOut, bool> predicate)
    {
        return new Future<IList<TIn>, IEnumerable<TOut>>(value =>
        {
            return future.Next(value).Where(predicate);
        });
    }

    public static IFuture<List<TIn>, IEnumerable<TOut>> Where<TIn, TOut>(this IFuture<List<TIn>, List<TOut>> future, Func<TOut, bool> predicate)
    {
        return new Future<List<TIn>, IEnumerable<TOut>>(value =>
        {
            return future.Next(value).Where(predicate);
        });
    }

    public static IFuture<List<TIn>, IEnumerable<TOut>> Where<TIn, TOut>(this IFuture<List<TIn>, IEnumerable<TOut>> future, Func<TOut, bool> predicate)
    {
        return new Future<List<TIn>, IEnumerable<TOut>>(value =>
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