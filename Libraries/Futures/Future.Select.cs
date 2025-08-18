namespace Futures;

public static partial class Future
{
    public static Future<T> Select<T, TOut>(Func<T, T> select, CancellationToken cancellation = default)
    {
        return new Future<T>(select, cancellation);
    }

    public static Future<T> Select<T, TOut>(Func<T, Task<T>> select, CancellationToken cancellation = default)
    {
        return new Future<T>(select, cancellation);
    }

    public static Future<T, TOut> Select<T, TOut>(Func<T, TOut> select, CancellationToken cancellation = default)
    {
        return new Future<T, TOut>(select, cancellation);
    }

    public static Future<T, TOut> Select<T, TOut>(Func<T, Task<TOut>> select, CancellationToken cancellation = default)
    {
        return new Future<T, TOut>(select, cancellation);
    }
}