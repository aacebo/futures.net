namespace Futures.Operators;

public static partial class FutureOperatorExtensions
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
}