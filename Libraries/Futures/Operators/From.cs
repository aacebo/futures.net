namespace Futures.Operators;

public sealed class From<T>(Future<T> target) : IOperator<T>
{
    public Future<T> Invoke(Future<T> src)
    {
        target.Subscribe(src);
        return src;
    }
}

public sealed class From<T, TOut, TOther>(Future<T, TOther> target) : IOperator<T, TOut>
{
    public Future<T, TOut> Invoke(Future<T, TOut> src)
    {
        target.In.Subscribe(src);
        return src;
    }
}

public sealed class From<T1, T2, TOut, TOther>(Future<T1, T2, TOther> target) : IOperator<T1, T2, TOut>
{
    public Future<T1, T2, TOut> Invoke(Future<T1, T2, TOut> src)
    {
        target.In.Subscribe((sender, value) => src.Next(sender, value.Item1, value.Item2));
        return src;
    }
}

public static partial class FutureExtensions
{
    public static Future<T> From<T>(this Future<T> future, Future<T> target)
    {
        return future.Pipe(new From<T>(target));
    }

    public static Future<T> From<T, TOut>(this Future<T> future, Future<T, TOut> target)
    {
        return future.Pipe(new From<T>(target.In));
    }

    public static Future<T, TOut> From<T, TOut, TOther>(this Future<T, TOut> future, Future<T, TOther> target)
    {
        return future.Pipe(new From<T, TOut, TOther>(target));
    }

    public static Future<T1, T2, TOut> From<T1, T2, TOut, TOther>(this Future<T1, T2, TOut> future, Future<T1, T2, TOther> target)
    {
        return future.Pipe(new From<T1, T2, TOut, TOther>(target));
    }
}