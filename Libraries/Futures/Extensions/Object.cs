namespace Futures.Extensions;

public static partial class ObjectExtensions
{
    public static IFuture<T, T> Pipe<T>(this T _)
    {
        return new Future<T, T>(value => value);
    }

    public static IFuture<T, T> Pipe<T>(this T _, Func<T, T> next)
    {
        return new Future<T, T>(value =>
        {
            return next(value);
        });
    }
}