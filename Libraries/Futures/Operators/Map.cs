namespace Futures.Operators;

public sealed class Map<T, TNext>(Fn<T, TNext> selector) : ITransformer<T, TNext>
{
    public Future<T, TNext> Invoke(Future<T> src)
    {
        return new Future<T, TNext>(value => selector.Invoke(src.Select(value)));
    }
}

public sealed class Map<T, TOut, TNext>(Fn<TOut, TNext> selector) : ITransformer<T, TOut, TNext>
{
    public Future<T, TNext> Invoke(Future<T, TOut> src)
    {
        return new Future<T, TNext>(value => selector.Invoke(src.Select(value)));
    }
}

public sealed class Map<T1, T2, TOut, TNext>(Fn<TOut, TNext> selector) : ITransformer<T1, T2, TOut, TNext>
{
    public Future<T1, T2, TNext> Invoke(Future<T1, T2, TOut> src)
    {
        return new Future<T1, T2, TNext>((a, b) => selector.Invoke(src.Select(a, b)));
    }
}

public static partial class FutureExtensions
{
    public static Future<T, TNext> Map<T, TNext>(this Future<T> future, Func<T, TNext> select)
    {
        return future.Pipe(new Map<T, TNext>(select));
    }

    public static Future<T, TNext> Map<T, TNext>(this Future<T> future, Func<T, Task<TNext>> select)
    {
        return future.Pipe(new Map<T, TNext>(select));
    }
}

public static partial class FutureExtensions
{
    public static Future<T, TNext> Map<T, TOut, TNext>(this Future<T, TOut> future, Func<TOut, TNext> select)
    {
        return future.Pipe(new Map<T, TOut, TNext>(select));
    }

    public static Future<T, TNext> Map<T, TOut, TNext>(this Future<T, TOut> future, Func<TOut, Task<TNext>> select)
    {
        return future.Pipe(new Map<T, TOut, TNext>(select));
    }
}

public static partial class FutureExtensions
{
    public static Future<T1, T2, TNext> Map<T1, T2, TOut, TNext>(this Future<T1, T2, TOut> future, Func<TOut, TNext> select)
    {
        return future.Pipe(new Map<T1, T2, TOut, TNext>(select));
    }

    public static Future<T1, T2, TNext> Map<T1, T2, TOut, TNext>(this Future<T1, T2, TOut> future, Func<TOut, Task<TNext>> select)
    {
        return future.Pipe(new Map<T1, T2, TOut, TNext>(select));
    }
}