namespace Futures.Operators;

public static partial class Future
{
    public static Future<T, TOut> Join<T, TOut>(Future<T> a, Future<T, TOut> b)
    {
        return new Future<T, TOut>(value => b.Next(a.Next(value)));
    }

    public static Future<T, TNext> Join<T, TOut, TNext>(Future<T, TOut> a, Future<TOut, TNext> b)
    {
        return new Future<T, TNext>(value => b.Next(a.Next(value)));
    }
}

public sealed partial class Join<T, TOut>(Future<T, TOut> other) : ITransformer<T, TOut>
{
    public Future<T, TOut> Invoke(Future<T> src)
    {
        return Future.Join(src, other);
    }
}

public sealed partial class Join<T, TOut, TNext>(Future<TOut, TNext> other) : ITransformer<T, TOut, TNext>
{
    public Future<T, TNext> Invoke(Future<T, TOut> src)
    {
        return Future.Join(src, other);
    }
}

public static partial class FutureExtensions
{
    public static Future<T, TOut> Join<T, TOut>(this Future<T> future, Future<T, TOut> other)
    {
        return future.Pipe(new Join<T, TOut>(other));
    }

    public static Future<T, TNext> Join<T, TOut, TNext>(this Future<T, TOut> future, Future<TOut, TNext> other)
    {
        return future.Pipe(new Join<T, TOut, TNext>(other));
    }
}