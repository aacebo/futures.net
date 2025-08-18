namespace Futures.Operators;

public static partial class FutureExtensions
{
    public static Future<T> Run<T>(this Future<T> future, Action<Future<T>> action)
    {
        action(future);
        return future;
    }

    public static Future<T> Run<T>(this Future<T> future, Func<Future<T>, Task> action)
    {
        action(future).ConfigureAwait(false);
        return future;
    }
}

public static partial class FutureExtensions
{
    public static Future<T, TOut> Run<T, TOut>(this Future<T, TOut> future, Action<Future<T, TOut>> action)
    {
        action(future);
        return future;
    }

    public static Future<T, TOut> Run<T, TOut>(this Future<T, TOut> future, Func<Future<T, TOut>, Task> action)
    {
        action(future).ConfigureAwait(false);
        return future;
    }
}

public static partial class FutureExtensions
{
    public static Future<T1, T2, TOut> Run<T1, T2, TOut>(this Future<T1, T2, TOut> future, Action<Future<T1, T2, TOut>> action)
    {
        action(future);
        return future;
    }

    public static Future<T1, T2, TOut> Run<T1, T2, TOut>(this Future<T1, T2, TOut> future, Func<Future<T1, T2, TOut>, Task> action)
    {
        action(future).ConfigureAwait(false);
        return future;
    }
}