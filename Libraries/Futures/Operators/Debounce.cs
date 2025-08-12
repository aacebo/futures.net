namespace Futures.Operators;

public static partial class FutureExtensions
{
    public static ITopic<T> Debounce<T>(this ITopic<T> future, TimeSpan time)
    {
        CancellationTokenSource? cancellation = new();

        return new Future<T>((value, consumer) =>
        {
            cancellation.Cancel();
            cancellation = new();

            _ = Task.Delay(time, cancellation.Token).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    return;
                }

                future.Next(value);
                consumer.Next(value);
                consumer.Complete();
            });
        }, future.Token);
    }

    public static ITopic<T> Debounce<T>(this ITopic<T> future, int ms = 200)
    {
        return Debounce(future, TimeSpan.FromMilliseconds(ms));
    }
}

public static partial class FutureExtensions
{
    public static ITransformer<T, TOut> Debounce<T, TOut>(this ITransformer<T, TOut> future, TimeSpan time)
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
        }, future.Token);
    }

    public static ITransformer<T, TOut> Debounce<T, TOut>(this ITransformer<T, TOut> future, int ms = 200)
    {
        return Debounce(future, TimeSpan.FromMilliseconds(ms));
    }
}

public static partial class FutureExtensions
{
    public static ITransformer<T1, T2, TOut> Debounce<T1, T2, TOut>(this ITransformer<T1, T2, TOut> future, TimeSpan time)
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
        }, future.Token);
    }

    public static ITransformer<T1, T2, TOut> Debounce<T1, T2, TOut>(this ITransformer<T1, T2, TOut> future, int ms = 200)
    {
        return Debounce(future, TimeSpan.FromMilliseconds(ms));
    }
}