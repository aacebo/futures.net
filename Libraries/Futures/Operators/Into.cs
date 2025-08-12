namespace Futures.Operators;

public static partial class FutureExtensions
{
    public static IFuture<T> Into<T>(this ITopic<T> future, ITopic<T> next)
    {
        return future.Map(value =>
        {
            next.Next(value);
            return value;
        });
    }

    public static IFuture<T> Into<T, TNext>(this ITopic<T> future, IStream<T, TNext> next)
    {
        return future.Map(value =>
        {
            next.Next(value);
            return value;
        });
    }
}

public static partial class FutureExtensions
{
    public static IStream<T, TOut> Into<T, TOut, TNext>(this IStream<T, TOut> future, ITopic<TOut> next)
    {
        return future.Map(value =>
        {
            next.Next(value);
            return value;
        });
    }

    public static IStream<T, TOut> Into<T, TOut, TNext>(this IStream<T, TOut> future, IStream<TOut, TNext> next)
    {
        return future.Map(value =>
        {
            next.Next(value);
            return value;
        });
    }
}

public static partial class FutureExtensions
{
    public static IStream<T1, T2, TOut> Into<T1, T2, TOut, TNext>(this IStream<T1, T2, TOut> future, ITopic<TOut> next)
    {
        return future.Map(value =>
        {
            next.Next(value);
            return value;
        });
    }

    public static IStream<T1, T2, TOut> Into<T1, T2, TOut, TNext>(this IStream<T1, T2, TOut> future, IStream<TOut, TNext> next)
    {
        return future.Map(value =>
        {
            next.Next(value);
            return value;
        });
    }
}