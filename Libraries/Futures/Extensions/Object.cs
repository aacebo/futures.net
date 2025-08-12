namespace Futures.Extensions;

public static partial class ObjectExtensions
{
    public static IFuture<T> Pipe<T>(this T value)
    {
        var future = new Future<T>(value);
        return future;
    }

    public static IFuture<TOut> Pipe<T, TOut>(this T value, Func<T, TOut> next)
    {
        var future = new Future<T, TOut>(next);
        future.Next(value);
        return future;
    }
}