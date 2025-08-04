namespace Futures.Operators;

public static partial class OperatorExtensions
{
    public static IFuture<TIn, TOut> Do<TIn, TOut>(this IFuture<TIn, TOut> future, Action<TOut> next)
    {
        return new Future<TIn, TOut>(value =>
        {
            var output = future.Next(value);
            next(output);
            return output;
        });
    }

    public static IFuture<T1, T2, TOut> Do<T1, T2, TOut>(this IFuture<T1, T2, TOut> future, Action<TOut> next)
    {
        return new Future<T1, T2, TOut>((a, b) =>
        {
            var output = future.Next(a, b);
            next(output);
            return output;
        });
    }
}