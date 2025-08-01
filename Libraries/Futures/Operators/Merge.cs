namespace Futures.Operators;

public static partial class OperatorExtensions
{
    public static IFuture<(TInA, TInB), (TOutA, TOutB)> Merge<TInA, TOutA, TInB, TOutB>(this IFuture<TInA, TOutA> a, IFuture<TInB, TOutB> b)
    {
        return new Future<(TInA, TInB), (TOutA, TOutB)>(value =>
        {
            return (a.Next(value.Item1), b.Next(value.Item2));
        });
    }
}