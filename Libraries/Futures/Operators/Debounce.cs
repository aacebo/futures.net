namespace Futures.Operators;

public static partial class FutureOperatorExtensions
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

    public static IFuture<TIn, TOut> Debounce<TIn, TOut>(this IFuture<TIn, TOut> future, int ms = 200)
    {
        return Debounce(future, TimeSpan.FromMilliseconds(ms));
    }
}