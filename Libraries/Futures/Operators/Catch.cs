namespace Futures.Operators;

public static partial class OperatorExtensions
{
    public static IFuture<TIn, TOut> Catch<TIn, TOut>(this IFuture<TIn, TOut> future, Func<Exception, TIn, TOut> next)
    {
        return new Future<TIn, TOut>(value =>
        {
            try
            {
                return future.Next(value);
            }
            catch (Exception ex)
            {
                return next(ex, value);
            }
        });
    }

    public static IFuture<T1, T2, TOut> Catch<T1, T2, TOut>(this IFuture<T1, T2, TOut> future, Func<Exception, T1, T2, TOut> next)
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
        });
    }

    public static IFuture<TIn, TOut> Catch<TIn, TOut>(this IFuture<TIn, TOut> future, Func<Exception, TIn, Task<TOut>> next)
    {
        return new Future<TIn, TOut>(value =>
        {
            try
            {
                return Task.FromResult(future.Next(value));
            }
            catch (Exception ex)
            {
                return next(ex, value);
            }
        });
    }

    public static IFuture<T1, T2, TOut> Catch<T1, T2, TOut>(this IFuture<T1, T2, TOut> future, Func<Exception, T1, T2, Task<TOut>> next)
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
        });
    }

    public static IFuture<TIn, TOut> Catch<TIn, TOut>(this IFuture<TIn, TOut> future, Action<Exception, TIn> next)
    {
        return new Future<TIn, TOut>(value =>
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
        });
    }

    public static IFuture<T1, T2, TOut> Catch<T1, T2, TOut>(this IFuture<T1, T2, TOut> future, Action<Exception, T1, T2> next)
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
        });
    }

    public static IFuture<TIn, TOut> Catch<TIn, TOut>(this IFuture<TIn, TOut> future, Func<Exception, TIn, Task> next)
    {
        return new Future<TIn, TOut>(async value =>
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
        });
    }

    public static IFuture<T1, T2, TOut> Catch<T1, T2, TOut>(this IFuture<T1, T2, TOut> future, Func<Exception, T1, T2, Task> next)
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
        });
    }
}