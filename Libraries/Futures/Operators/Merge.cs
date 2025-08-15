namespace Futures.Operators;

public static partial class Futures
{
    public static Future<T> Merge<T>(Future<T> a, Future<T> b)
    {
        return new Future<T>((value, dest) =>
        {
            dest.Next(a.Next(value));
            dest.Next(b.Next(value));
            dest.Complete();
        });
    }

    public static Future<T, TOut> Merge<T, TOut>(Future<T, TOut> a, Future<T, TOut> b)
    {
        return new Future<T, TOut>((value, dest) =>
        {
            dest.Next(a.Next(value));
            dest.Next(b.Next(value));
            dest.Complete();
        });
    }

    public static Future<T1, T2, TOut> Merge<T1, T2, TOut>(Future<T1, T2, TOut> a, Future<T1, T2, TOut> b)
    {
        return new Future<T1, T2, TOut>((one, two, dest) =>
        {
            dest.Next(a.Next(one, two));
            dest.Next(b.Next(one, two));
            dest.Complete();
        });
    }
}

public sealed class Merge<T>(Future<T> other) : IOperator<T>
{
    public Future<T> Invoke(Future<T> src)
    {
        return Futures.Merge(src, other);
    }
}

public sealed class Merge<T, TOut>(Future<T, TOut> other) : IOperator<T, TOut>
{
    public Future<T, TOut> Invoke(Future<T, TOut> src)
    {
        return Futures.Merge(src, other);
    }
}

public sealed class Merge<T1, T2, TOut>(Future<T1, T2, TOut> other) : IOperator<T1, T2, TOut>
{
    public Future<T1, T2, TOut> Invoke(Future<T1, T2, TOut> src)
    {
        return Futures.Merge(src, other);
    }
}

public static partial class FutureExtensions
{
    public static Future<T> Merge<T>(this Future<T> future, Future<T> other)
    {
        return future.Pipe(new Merge<T>(other));
    }

    public static Future<T, TOut> Merge<T, TOut>(this Future<T, TOut> future, Future<T, TOut> other)
    {
        return future.Pipe(new Merge<T, TOut>(other));
    }

    public static Future<T1, T2, TOut> Merge<T1, T2, TOut>(this Future<T1, T2, TOut> future, Future<T1, T2, TOut> other)
    {
        return future.Pipe(new Merge<T1, T2, TOut>(other));
    }
}