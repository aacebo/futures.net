namespace Futures.Operators;

public static partial class FutureOperatorExtensions
{
    public static IFuture<TIn, TOut> If<TIn, TOut>(this IFuture<TIn, TOut> future, Func<TIn, bool> predicate)
    {
        return new Future<TIn, TOut>(value =>
        {
            return predicate(value) ? future.Next(value) : future.Value;
        });
    }
}