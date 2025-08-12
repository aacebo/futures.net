namespace Futures.Operators;

public static partial class FutureExtensions
{
    public static ITopic<T> Retry<T>(this ITopic<T> future, int attempts = 3)
    {
        return new Future<T>(value =>
        {
            Exception? last = null;

            for (var i = 0; i < attempts; i++)
            {
                try
                {
                    future.Next(value);
                    return value;
                }
                catch (Exception ex)
                {
                    last = ex;
                }
            }

            throw last ?? new Exception("max retry attempts reached");
        }, future.Token);
    }
}

public static partial class FutureExtensions
{
    public static ITransformer<T, TOut> Retry<T, TOut>(this ITransformer<T, TOut> future, int attempts = 3)
    {
        return new Future<T, TOut>(value =>
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
        }, future.Token);
    }
}

public static partial class FutureExtensions
{
    public static ITransformer<T1, T2, TOut> Retry<T1, T2, TOut>(this ITransformer<T1, T2, TOut> future, int attempts = 3)
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
        }, future.Token);
    }
}