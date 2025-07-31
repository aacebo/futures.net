namespace Futures.Extensions;

public static partial class ObjectExtensions
{
    public static IFuture<T, T> Pipe<T>(this T _)
    {
        return new Future<T, T>(value => value);
    }

    public static IFuture<TIn, TOut> Pipe<TIn, TOut>(this TIn _, Func<TIn, TOut> next)
    {
        return new Future<TIn, TOut>(value =>
        {
            return next(value);
        });
    }
}