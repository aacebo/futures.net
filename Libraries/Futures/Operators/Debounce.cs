namespace Futures.Operators;

public static partial class FutureExtensions
{
    public static Future<T> Debounce<T>(this Future<T> future, TimeSpan time)
    {
        CancellationTokenSource? cancellation = new();

        return new Future<T>((value, producer) =>
        {
            cancellation.Cancel();
            cancellation = new();

            _ = Task.Delay(time, cancellation.Token).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    return;
                }

                producer.Next(future.Next(value));
                producer.Complete();
            });
        });
    }

    public static Future<T> Debounce<T>(this Future<T> future, int ms = 200)
    {
        return Debounce(future, TimeSpan.FromMilliseconds(ms));
    }
}

public static partial class FutureExtensions
{
    public static Future<T, TOut> Debounce<T, TOut>(this Future<T, TOut> future, TimeSpan time)
    {
        CancellationTokenSource? cancellation = new();

        return new Future<T, TOut>((value, producer) =>
        {
            cancellation.Cancel();
            cancellation = new();

            _ = Task.Delay(time, cancellation.Token).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    return;
                }

                producer.Next(future.Next(value));
                producer.Complete();
            });
        });
    }

    public static Future<T, TOut> Debounce<T, TOut>(this Future<T, TOut> future, int ms = 200)
    {
        return Debounce(future, TimeSpan.FromMilliseconds(ms));
    }
}

public static partial class FutureExtensions
{
    public static Future<T1, T2, TOut> Debounce<T1, T2, TOut>(this Future<T1, T2, TOut> future, TimeSpan time)
    {
        CancellationTokenSource? cancellation = new();

        return new Future<T1, T2, TOut>((a, b, producer) =>
        {
            cancellation.Cancel();
            cancellation = new();

            _ = Task.Delay(time, cancellation.Token).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    return;
                }

                producer.Next(future.Next(a, b));
                producer.Complete();
            });
        });
    }

    public static Future<T1, T2, TOut> Debounce<T1, T2, TOut>(this Future<T1, T2, TOut> future, int ms = 200)
    {
        return Debounce(future, TimeSpan.FromMilliseconds(ms));
    }
}