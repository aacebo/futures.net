namespace Futures.Operators;

public static partial class FutureExtensions
{
    public static ITopic<T> Catch<T>(this ITopic<T> future, Func<Exception, T, T> next)
    {
        return new Future<T>(value =>
        {
            try
            {
                future.Next(value);
                return future.Value;
            }
            catch (Exception ex)
            {
                return next(ex, value);
            }
        }, future.Token);
    }

    public static ITopic<T> Catch<T>(this ITopic<T> future, Func<Exception, T, Task<T>> next)
    {
        return new Future<T>(value =>
        {
            try
            {
                future.Next(value);
                return Task.FromResult(future.Value);
            }
            catch (Exception ex)
            {
                return next(ex, value);
            }
        }, future.Token);
    }

    public static ITopic<T> Catch<T>(this ITopic<T> future, Action<Exception, T> next)
    {
        return new Future<T>(value =>
        {
            try
            {
                future.Next(value);
                return Task.FromResult(value);
            }
            catch (Exception ex)
            {
                next(ex, value);
                return Task.FromResult(value);
            }
        }, future.Token);
    }

    public static ITopic<T> Catch<T>(this ITopic<T> future, Func<Exception, T, Task> next)
    {
        return new Future<T>(async value =>
        {
            try
            {
                future.Next(value);
                return value;
            }
            catch (Exception ex)
            {
                await next(ex, value);
                return value;
            }
        }, future.Token);
    }
}

public static partial class FutureExtensions
{
    public static IStream<T, TOut> Catch<T, TOut>(this IStream<T, TOut> future, Func<Exception, T, TOut> next)
    {
        return new Future<T, TOut>(value =>
        {
            try
            {
                return future.Next(value);
            }
            catch (Exception ex)
            {
                return next(ex, value);
            }
        }, future.Token);
    }

    public static IStream<T, TOut> Catch<T, TOut>(this IStream<T, TOut> future, Func<Exception, T, Task<TOut>> next)
    {
        return new Future<T, TOut>(value =>
        {
            try
            {
                return Task.FromResult(future.Next(value));
            }
            catch (Exception ex)
            {
                return next(ex, value);
            }
        }, future.Token);
    }

    public static IStream<T, TOut> Catch<T, TOut>(this IStream<T, TOut> future, Action<Exception, T> next)
    {
        return new Future<T, TOut>(value =>
        {
            try
            {
                return Task.FromResult(future.Next(value));
            }
            catch (Exception ex)
            {
                next(ex, value);
                return Task.FromResult(future.Value);
            }
        }, future.Token);
    }

    public static IStream<T, TOut> Catch<T, TOut>(this IStream<T, TOut> future, Func<Exception, T, Task> next)
    {
        return new Future<T, TOut>(async value =>
        {
            try
            {
                return future.Next(value);
            }
            catch (Exception ex)
            {
                await next(ex, value);
                return future.Value;
            }
        }, future.Token);
    }
}

public static partial class FutureExtensions
{
    public static IStream<T1, T2, TOut> Catch<T1, T2, TOut>(this IStream<T1, T2, TOut> future, Func<Exception, T1, T2, TOut> next)
    {
        return new Future<T1, T2, TOut>((a, b) =>
        {
            try
            {
                return future.Next(a, b);
            }
            catch (Exception ex)
            {
                return next(ex, a, b);
            }
        }, future.Token);
    }

    public static IStream<T1, T2, TOut> Catch<T1, T2, TOut>(this IStream<T1, T2, TOut> future, Func<Exception, T1, T2, Task<TOut>> next)
    {
        return new Future<T1, T2, TOut>((a, b) =>
        {
            try
            {
                return Task.FromResult(future.Next(a, b));
            }
            catch (Exception ex)
            {
                return next(ex, a, b);
            }
        }, future.Token);
    }

    public static IStream<T1, T2, TOut> Catch<T1, T2, TOut>(this IStream<T1, T2, TOut> future, Action<Exception, T1, T2> next)
    {
        return new Future<T1, T2, TOut>((a, b) =>
        {
            try
            {
                return Task.FromResult(future.Next(a, b));
            }
            catch (Exception ex)
            {
                next(ex, a, b);
                return Task.FromResult(future.Value);
            }
        }, future.Token);
    }

    public static IStream<T1, T2, TOut> Catch<T1, T2, TOut>(this IStream<T1, T2, TOut> future, Func<Exception, T1, T2, Task> next)
    {
        return new Future<T1, T2, TOut>(async (a, b) =>
        {
            try
            {
                return future.Next(a, b);
            }
            catch (Exception ex)
            {
                await next(ex, a, b);
                return future.Value;
            }
        }, future.Token);
    }
}