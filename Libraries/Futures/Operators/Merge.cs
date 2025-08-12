namespace Futures.Operators;

public static partial class FutureExtensions
{
    public static ITransformer<(TInA, TInB), (TOutA, TOutB)> Merge<TInA, TOutA, TInB, TOutB>(this ITransformer<TInA, TOutA> a, ITransformer<TInB, TOutB> b)
    {
        return new Future<(TInA, TInB), (TOutA, TOutB)>(value =>
        {
            return (a.Next(value.Item1), b.Next(value.Item2));
        });
    }
}