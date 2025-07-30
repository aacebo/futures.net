namespace Futures.Operators;

public static partial class FutureOperatorExtensions
{
    public static IFuture<TIn, TOut> Debounce<TIn, TOut>(this IFuture<TIn, TOut> future, TimeSpan time)
    {
        CancellationTokenSource? cancellation = null;

        return new Future<TIn, TOut>(value =>
        {
            cancellation?.Cancel(true);
            cancellation = new();

            return Task.Run(async () =>
            {
                var delay = Task.Delay(time, cancellation.Token);
                await delay;

                if (delay.IsCanceled)
                {
                    throw new CancelledException();
                }

                return future.Next(value);
            });
        });
    }
}