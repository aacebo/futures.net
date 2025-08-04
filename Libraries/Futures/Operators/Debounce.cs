namespace Futures.Operators;

public static partial class OperatorExtensions
{
    public static IFuture<TIn, TOut> Debounce<TIn, TOut>(this IFuture<TIn, TOut> future, TimeSpan time)
    {
        CancellationTokenSource? cancellation = new();

        return new Future<TIn, TOut>((value, subscriber) =>
        {
            cancellation.Cancel();
            cancellation = new();

            _ = Task.Delay(time, cancellation.Token).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    return;
                }

                subscriber.Next(future.Next(value));
                subscriber.Complete();
            });
        });
    }

    public static IFuture<T1, T2, TOut> Debounce<T1, T2, TOut>(this IFuture<T1, T2, TOut> future, TimeSpan time)
    {
        CancellationTokenSource? cancellation = new();

        return new Future<T1, T2, TOut>((a, b, subscriber) =>
        {
            cancellation.Cancel();
            cancellation = new();

            _ = Task.Delay(time, cancellation.Token).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    return;
                }

                subscriber.Next(future.Next(a, b));
                subscriber.Complete();
            });
        });
    }

    public static IFuture<TIn, TOut> Debounce<TIn, TOut>(this IFuture<TIn, TOut> future, int ms = 200)
    {
        return Debounce(future, TimeSpan.FromMilliseconds(ms));
    }

    public static IFuture<T1, T2, TOut> Debounce<T1, T2, TOut>(this IFuture<T1, T2, TOut> future, int ms = 200)
    {
        return Debounce(future, TimeSpan.FromMilliseconds(ms));
    }
}