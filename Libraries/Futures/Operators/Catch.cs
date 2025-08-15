namespace Futures.Operators;

public sealed class Catch<T>(Fn<Exception, T, Future<T>, T> selector) : IOperator<T>
{
    public Future<T> Invoke(Future<T> src)
    {
        return new Future<T>((value, dest) =>
        {
            try
            {
                dest.Next(src.Next(value));
            }
            catch (Exception error)
            {
                dest.Next(selector.Invoke(error, value, Invoke(src)));
            }

            dest.Complete();
        });
    }
}

public sealed class Catch<T, TOut>(Fn<Exception, T, Future<T, TOut>, TOut> selector) : IOperator<T, TOut>
{
    public Future<T, TOut> Invoke(Future<T, TOut> src)
    {
        return new Future<T, TOut>((value, dest) =>
        {
            try
            {
                dest.Next(src.Next(value));
            }
            catch (Exception error)
            {
                dest.Next(selector.Invoke(error, value, Invoke(src)));
            }

            dest.Complete();
        });
    }
}

public sealed class Catch<T1, T2, TOut>(Fn<Exception, (T1, T2), Future<T1, T2, TOut>, TOut> selector) : IOperator<T1, T2, TOut>
{
    public Future<T1, T2, TOut> Invoke(Future<T1, T2, TOut> src)
    {
        return new Future<T1, T2, TOut>((a, b, dest) =>
        {
            try
            {
                dest.Next(src.Next(a, b));
            }
            catch (Exception error)
            {
                dest.Next(selector.Invoke(error, (a, b), Invoke(src)));
            }

            dest.Complete();
        });
    }
}

public static partial class FutureExtensions
{
    public static Future<T> Catch<T>(this Future<T> future, Func<Exception, T, Future<T>, T> select)
    {
        return future.Pipe(new Catch<T>(select));
    }

    public static Future<T> Catch<T>(this Future<T> future, Func<Exception, T, Future<T>, Task<T>> select)
    {
        return future.Pipe(new Catch<T>(select));
    }
}

public static partial class FutureExtensions
{
    public static Future<T, TOut> Catch<T, TOut>(this Future<T, TOut> future, Func<Exception, T, Future<T, TOut>, TOut> select)
    {
        return future.Pipe(new Catch<T, TOut>(select));
    }

    public static Future<T, TOut> Catch<T, TOut>(this Future<T, TOut> future, Func<Exception, T, Future<T, TOut>, Task<TOut>> select)
    {
        return future.Pipe(new Catch<T, TOut>(select));
    }
}

public static partial class FutureExtensions
{
    public static Future<T1, T2, TOut> Catch<T1, T2, TOut>(this Future<T1, T2, TOut> future, Func<Exception, (T1, T2), Future<T1, T2, TOut>, TOut> select)
    {
        return future.Pipe(new Catch<T1, T2, TOut>(select));
    }

    public static Future<T1, T2, TOut> Catch<T1, T2, TOut>(this Future<T1, T2, TOut> future, Func<Exception, (T1, T2), Future<T1, T2, TOut>, Task<TOut>> select)
    {
        return future.Pipe(new Catch<T1, T2, TOut>(select));
    }
}