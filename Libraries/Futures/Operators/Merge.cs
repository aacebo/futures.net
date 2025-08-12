namespace Futures.Operators;

public static partial class FutureExtensions
{
    public static IStream<(TInA, TInB), (TOutA, TOutB)> Merge<TInA, TOutA, TInB, TOutB>(this IStream<TInA, TOutA> a, IStream<TInB, TOutB> b)
    {
        return new Future<(TInA, TInB), (TOutA, TOutB)>(value =>
        {
            return (a.Next(value.Item1), b.Next(value.Item2));
        });
    }
}