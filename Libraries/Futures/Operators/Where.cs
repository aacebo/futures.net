namespace Futures.Operators;

public static partial class FutureOperatorExtensions
{
    public static IFuture<TIn, TOut> Where<TIn, TOut, TItem>(this IFuture<TIn, TOut> future, Func<TItem, bool> predicate) where TOut : IEnumerable<TItem>
    {
        return new Future<TIn, TOut>(value =>
        {
            var output = future.Next(value).Where(predicate);
            return (TOut)output;
        });
    }
}