namespace Futures.Operators;

public sealed class Retry<T>(int attempts = 3, int delay = 0) : IOperator<T>
{
    public Future<T> Invoke(Future<T> src)
    {
        return new Future<T>((value, dest) =>
        {
            List<Exception> errors = [];

            for (var i = 0; i < attempts; i++)
            {
                try
                {
                    dest.Next(src.Next(value));
                    dest.Complete();
                    return;
                }
                catch (Exception error)
                {
                    errors.Add(error);
                }

                Task.Delay(delay).Wait();
            }

            dest.Error(new AggregateException(errors));
        });
    }
}

public sealed class Retry<T, TOut>(int attempts = 3, int delay = 0) : IOperator<T, TOut>
{
    public Future<T, TOut> Invoke(Future<T, TOut> src)
    {
        return new Future<T, TOut>((value, dest) =>
        {
            List<Exception> errors = [];

            for (var i = 0; i < attempts; i++)
            {
                try
                {
                    dest.Next(src.Next(value));
                    dest.Complete();
                    return;
                }
                catch (Exception error)
                {
                    errors.Add(error);
                }

                Task.Delay(delay).Wait();
            }

            dest.Error(new AggregateException(errors));
        });
    }
}

public sealed class Retry<T1, T2, TOut>(int attempts = 3, int delay = 0) : IOperator<T1, T2, TOut>
{
    public Future<T1, T2, TOut> Invoke(Future<T1, T2, TOut> src)
    {
        return new Future<T1, T2, TOut>((a, b, dest) =>
        {
            List<Exception> errors = [];

            for (var i = 0; i < attempts; i++)
            {
                try
                {
                    dest.Next(src.Next(a, b));
                    dest.Complete();
                    return;
                }
                catch (Exception error)
                {
                    errors.Add(error);
                }

                Task.Delay(delay).Wait();
            }

            dest.Error(new AggregateException(errors));
        });
    }
}

public static partial class FutureExtensions
{
    public static Future<T> Retry<T>(this Future<T> future, int attempts = 3, int delay = 0)
    {
        return future.Pipe(new Retry<T>(attempts, delay));
    }

    public static Future<T, TOut> Retry<T, TOut>(this Future<T, TOut> future, int attempts = 3, int delay = 0)
    {
        return future.Pipe(new Retry<T, TOut>(attempts, delay));
    }

    public static Future<T1, T2, TOut> Retry<T1, T2, TOut>(this Future<T1, T2, TOut> future, int attempts = 3, int delay = 0)
    {
        return future.Pipe(new Retry<T1, T2, TOut>(attempts, delay));
    }
}