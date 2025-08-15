namespace Futures.Operators;

public sealed class Do<T>(Fn<T> selector) : IOperator<T>
{
    public Future<T> Invoke(Future<T> src)
    {
        return new Future<T>((value, dest) =>
        {
            var @out = src.Next(value);
            selector.Invoke(@out);
            dest.Next(@out);
            dest.Complete();
        });
    }
}

public sealed class Do<T, TOut>(Fn<TOut> selector) : IOperator<T, TOut>
{
    public Future<T, TOut> Invoke(Future<T, TOut> src)
    {
        return new Future<T, TOut>((value, dest) =>
        {
            var @out = src.Next(value);
            selector.Invoke(@out);
            dest.Next(@out);
            dest.Complete();
        });
    }
}

public sealed class Do<T1, T2, TOut>(Fn<TOut> selector) : IOperator<T1, T2, TOut>
{
    public Future<T1, T2, TOut> Invoke(Future<T1, T2, TOut> src)
    {
        return new Future<T1, T2, TOut>((a, b, dest) =>
        {
            var @out = src.Next(a, b);
            selector.Invoke(@out);
            dest.Next(@out);
            dest.Complete();
        });
    }
}

public static partial class FutureExtensions
{
    public static Future<T> Do<T>(this Future<T> future, Action<T> select)
    {
        return future.Pipe(new Do<T>(select));
    }

    public static Future<T> Do<T>(this Future<T> future, Func<T, Task> select)
    {
        return future.Pipe(new Do<T>(select));
    }
}

public static partial class FutureExtensions
{
    public static Future<T, TOut> Do<T, TOut>(this Future<T, TOut> future, Action<TOut> select)
    {
        return future.Pipe(new Do<T, TOut>(select));
    }

    public static Future<T, TOut> Do<T, TOut>(this Future<T, TOut> future, Func<TOut, Task> select)
    {
        return future.Pipe(new Do<T, TOut>(select));
    }
}

public static partial class FutureExtensions
{
    public static Future<T1, T2, TOut> Do<T1, T2, TOut>(this Future<T1, T2, TOut> future, Action<TOut> select)
    {
        return future.Pipe(new Do<T1, T2, TOut>(select));
    }

    public static Future<T1, T2, TOut> Do<T1, T2, TOut>(this Future<T1, T2, TOut> future, Func<TOut, Task> select)
    {
        return future.Pipe(new Do<T1, T2, TOut>(select));
    }
}