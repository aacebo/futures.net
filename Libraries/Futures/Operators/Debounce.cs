namespace Futures.Operators;

public static partial class FutureOperatorExtensions
{
    public static IFuture<TIn, TOut> Debounce<TIn, TOut>(this IFuture<TIn, TOut> future, TimeSpan time)
    {
        CancellationTokenSource? cancellation = null;

        return new Future<TIn, TOut>(value =>
        {
            cancellation?.Cancel();
            cancellation = new();

            return Task.Run(async () =>
            {
                var delay = Task.Delay(time, cancellation.Token);
                await delay;

                if (cancellation.IsCancellationRequested)
                {
                    throw new CancelledException();
                }

                return future.Next(value);
            });
        });
    }
}