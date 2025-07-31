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
}