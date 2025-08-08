namespace Futures.Operators;

public static partial class FutureExtensions
{
    public static Future<T> Do<T>(this Future<T> future, Action<T> next)
    {
        return new Future<T>(value =>
        {
            var output = future.Next(value);
            next(output);
            return output;
        }, future.Token);
    }

    public static Future<T> Do<T>(this Future<T> future, Func<T, Task> next)
    {
        return new Future<T>(value =>
        {
            var output = future.Next(value);
            next(output).ConfigureAwait(false).GetAwaiter().GetResult();
            return output;
        }, future.Token);
    }
}

public static partial class FutureExtensions
{
    public static Future<T, TOut> Do<T, TOut>(this Future<T, TOut> future, Action<TOut> next)
    {
        return new Future<T, TOut>(value =>
        {
            var output = future.Next(value);
            next(output);
            return output;
        }, future.Token);
    }

    public static Future<T, TOut> Do<T, TOut>(this Future<T, TOut> future, Func<TOut, Task> next)
    {
        return new Future<T, TOut>(value =>
        {
            var output = future.Next(value);
            next(output).ConfigureAwait(false).GetAwaiter().GetResult();
            return output;
        }, future.Token);
    }
}

public static partial class FutureExtensions
{
    public static Future<T1, T2, TOut> Do<T1, T2, TOut>(this Future<T1, T2, TOut> future, Action<TOut> next)
    {
        return new Future<T1, T2, TOut>((a, b) =>
        {
            var output = future.Next(a, b);
            next(output);
            return output;
        }, future.Token);
    }

    public static Future<T1, T2, TOut> Do<T1, T2, TOut>(this Future<T1, T2, TOut> future, Func<TOut, Task> next)
    {
        return new Future<T1, T2, TOut>((a, b) =>
        {
            var output = future.Next(a, b);
            next(output).ConfigureAwait(false).GetAwaiter().GetResult();
            return output;
        }, future.Token);
    }
}