namespace Futures.Operators;

public sealed class Split<T, TNext>(Fn<T, TNext> selector) : ITransformer<T, TNext>
{
    public Future<T, TNext> Invoke(Future<T> src)
    {
        return new Future<T, TNext>(selector.Invoke);
    }
}

public sealed class Split<T, TOut, TNext>(Fn<T, TNext> selector) : ITransformer<T, TOut, TNext>
{
    public Future<T, TNext> Invoke(Future<T, TOut> src)
    {
        return new Future<T, TNext>(selector.Invoke);
    }
}

public sealed class Split<T1, T2, TOut, TNext>(Fn<T1, T2, TNext> selector) : ITransformer<T1, T2, TOut, TNext>
{
    public Future<T1, T2, TNext> Invoke(Future<T1, T2, TOut> src)
    {
        return new Future<T1, T2, TNext>(selector.Invoke);
    }
}

public static partial class FutureExtensions
{
    public static Future<T, TNext> Split<T, TNext>(this Future<T> future, Func<T, TNext> select)
    {
        return future.Pipe(new Split<T, TNext>(select));
    }

    public static Future<T, TNext> Split<T, TNext>(this Future<T> future, Func<T, Task<TNext>> select)
    {
        return future.Pipe(new Split<T, TNext>(select));
    }
}

public static partial class FutureExtensions
{
    public static Future<T, TNext> Split<T, TOut, TNext>(this Future<T, TOut> future, Func<T, TNext> select)
    {
        return future.Pipe(new Split<T, TOut, TNext>(select));
    }

    public static Future<T, TNext> Split<T, TOut, TNext>(this Future<T, TOut> future, Func<T, Task<TNext>> select)
    {
        return future.Pipe(new Split<T, TOut, TNext>(select));
    }
}

public static partial class FutureExtensions
{
    public static Future<T1, T2, TNext> Split<T1, T2, TOut, TNext>(this Future<T1, T2, TOut> future, Func<T1, T2, TNext> select)
    {
        return future.Pipe(new Split<T1, T2, TOut, TNext>(select));
    }

    public static Future<T1, T2, TNext> Split<T1, T2, TOut, TNext>(this Future<T1, T2, TOut> future, Func<T1, T2, Task<TNext>> select)
    {
        return future.Pipe(new Split<T1, T2, TOut, TNext>(select));
    }
}