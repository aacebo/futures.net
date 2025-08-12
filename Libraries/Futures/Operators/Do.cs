namespace Futures.Operators;

public static partial class FutureExtensions
{
    public static ITopic<T> Do<T>(this ITopic<T> future, Action<T> next)
    {
        return new Future<T>(value =>
        {
            future.Next(value);
            next(value);
            return value;
        }, future.Token);
    }

    public static ITopic<T> Do<T>(this ITopic<T> future, Func<T, Task> next)
    {
        return new Future<T>(value =>
        {
            future.Next(value);
            next(value).ConfigureAwait(false).GetAwaiter().GetResult();
            return value;
        }, future.Token);
    }
}

public static partial class FutureExtensions
{
    public static ITransformer<T, TOut> Do<T, TOut>(this ITransformer<T, TOut> future, Action<TOut> next)
    {
        return new Future<T, TOut>(value =>
        {
            var output = future.Next(value);
            next(output);
            return output;
        }, future.Token);
    }

    public static ITransformer<T, TOut> Do<T, TOut>(this ITransformer<T, TOut> future, Func<TOut, Task> next)
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
    public static ITransformer<T1, T2, TOut> Do<T1, T2, TOut>(this ITransformer<T1, T2, TOut> future, Action<TOut> next)
    {
        return new Future<T1, T2, TOut>((a, b) =>
        {
            var output = future.Next(a, b);
            next(output);
            return output;
        }, future.Token);
    }

    public static ITransformer<T1, T2, TOut> Do<T1, T2, TOut>(this ITransformer<T1, T2, TOut> future, Func<TOut, Task> next)
    {
        return new Future<T1, T2, TOut>((a, b) =>
        {
            var output = future.Next(a, b);
            next(output).ConfigureAwait(false).GetAwaiter().GetResult();
            return output;
        }, future.Token);
    }
}