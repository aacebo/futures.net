namespace Futures.Operators;

public static partial class FutureOperatorExtensions
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
}