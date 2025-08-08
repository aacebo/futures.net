namespace Futures.Operators;

public static partial class FutureExtensions
{
    public static Future<T> Into<T>(this Future<T> future, Future<T> next)
    {
        return future.Pipe(value =>
        {
            next.Next(value);
            return value;
        });
    }

    public static Future<T> Into<T, TNext>(this Future<T> future, Future<T, TNext> next)
    {
        return future.Pipe(value =>
        {
            next.Next(value);
            return value;
        });
    }
}

public static partial class FutureExtensions
{
    public static Future<T, TOut> Into<T, TOut, TNext>(this Future<T, TOut> future, Future<TOut> next)
    {
        return future.Pipe(value =>
        {
            next.Next(value);
            return value;
        });
    }

    public static Future<T, TOut> Into<T, TOut, TNext>(this Future<T, TOut> future, Future<TOut, TNext> next)
    {
        return future.Pipe(value =>
        {
            next.Next(value);
            return value;
        });
    }
}

public static partial class FutureExtensions
{
    public static Future<T1, T2, TOut> Into<T1, T2, TOut, TNext>(this Future<T1, T2, TOut> future, Future<TOut> next)
    {
        return future.Pipe(value =>
        {
            next.Next(value);
            return value;
        });
    }

    public static Future<T1, T2, TOut> Into<T1, T2, TOut, TNext>(this Future<T1, T2, TOut> future, Future<TOut, TNext> next)
    {
        return future.Pipe(value =>
        {
            next.Next(value);
            return value;
        });
    }
}