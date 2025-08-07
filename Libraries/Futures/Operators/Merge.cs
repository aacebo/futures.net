namespace Futures.Operators;

public static partial class FutureExtensions
{
    public static Future<(T1, T2)> Merge<T1, T2>(this Future<T1> a, Future<T2> b)
    {
        return new Future<(T1, T2)>(value =>
        {
            return (a.Next(value.Item1), b.Next(value.Item2));
        });
    }

    public static Future<(TInA, TInB), (TOutA, TOutB)> Merge<TInA, TOutA, TInB, TOutB>(this Future<TInA, TOutA> a, Future<TInB, TOutB> b)
    {
        return new Future<(TInA, TInB), (TOutA, TOutB)>(value =>
        {
            return (a.Next(value.Item1), b.Next(value.Item2));
        });
    }
}