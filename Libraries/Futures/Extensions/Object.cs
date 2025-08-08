namespace Futures.Extensions;

public static partial class ObjectExtensions
{
    public static Future<T, T> Pipe<T>(this T value)
    {
        var future = new Future<T, T>(value => value);
        future.Next(value);
        return future;
    }

    public static Future<T, TOut> Pipe<T, TOut>(this T value, Func<T, TOut> next)
    {
        var future = new Future<T, TOut>(next);
        future.Next(value);
        return future;
    }
}