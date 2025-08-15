namespace Futures.Operators;

public static partial class Futures
{
    public static Future<T, TOut> Join<T, TOut>(Future<T> a, Future<T, TOut> b)
    {
        return new Future<T, TOut>((value, dest) =>
        {
            var @out = b.Next(a.Next(value));
            dest.Next(@out);
            dest.Complete();
        });
    }

    public static Future<T, TOut> Join<T, TOut>(Future<T, TOut> a, Future<TOut> b)
    {
        return new Future<T, TOut>((value, dest) =>
        {
            var @out = b.Next(a.Next(value));
            dest.Next(@out);
            dest.Complete();
        });
    }

    public static Future<T, TNext> Join<T, TOut, TNext>(Future<T, TOut> a, Future<TOut, TNext> b)
    {
        return new Future<T, TNext>((value, dest) =>
        {
            var @out = b.Next(a.Next(value));
            dest.Next(@out);
            dest.Complete();
        });
    }
}