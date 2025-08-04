namespace Futures.Operators;

public static partial class OperatorExtensions
{
    public static IFuture<TIn, TOut> Into<TIn, TOut, TNext>(this IFuture<TIn, TOut> future, IFuture<TOut, TNext> next)
    {
        return new Future<TIn, TOut>(value =>
        {
            var output = future.Next(value);
            next.Next(output);
            return output;
        });
    }

    public static IFuture<T1, T2, TOut> Into<T1, T2, TOut, TNext>(this IFuture<T1, T2, TOut> future, IFuture<TOut, TNext> next)
    {
        return new Future<T1, T2, TOut>((a, b) =>
        {
            var output = future.Next(a, b);
            next.Next(output);
            return output;
        });
    }
}