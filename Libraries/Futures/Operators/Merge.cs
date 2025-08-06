namespace Futures.Operators;

public static partial class FutureExtensions
{
    public static Future<(T1, T2)> Merge<T1, T2>(this Future<T1> future, Future<T2> other)
    {
        return future.Pipe(a =>
        {
            return other.Pipe(b => (a, b));
        });
    }
}