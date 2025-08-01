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
}