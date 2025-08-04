namespace Futures.Operators;

public static partial class OperatorExtensions
{
    public static IFuture<TIn, TOut> Retry<TIn, TOut>(this IFuture<TIn, TOut> future, int attempts = 3)
    {
        return new Future<TIn, TOut>(value =>
        {
            Exception? last = null;

            for (var i = 0; i < attempts; i++)
            {
                try
                {
                    return future.Next(value);
                }
                catch (Exception ex)
                {
                    last = ex;
                }
            }

            throw last ?? new Exception("max retry attempts reached");
        });
    }

    public static IFuture<T1, T2, TOut> Retry<T1, T2, TOut>(this IFuture<T1, T2, TOut> future, int attempts = 3)
    {
        return new Future<T1, T2, TOut>((a, b) =>
        {
            Exception? last = null;

            for (var i = 0; i < attempts; i++)
            {
                try
                {
                    return future.Next(a, b);
                }
                catch (Exception ex)
                {
                    last = ex;
                }
            }

            throw last ?? new Exception("max retry attempts reached");
        });
    }
}