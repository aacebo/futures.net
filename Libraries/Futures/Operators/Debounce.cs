namespace Futures.Operators;

public sealed class Debounce<T> : IOperator<T>
{
    private readonly TimeSpan _time;

    public Debounce(TimeSpan time)
    {
        _time = time;
    }

    public IFuture<T> Invoke(IFuture<T> source)
    {
        return new Future<T>(destination =>
        {
            T? prev = default;
            CancellationTokenSource cancellation = new();

            source.Subscribe(new Subscriber<T>(destination)
            {
                OnNext = value =>
                {
                    prev = value;
                    cancellation.Cancel();
                    cancellation = new();

                    _ = Task.Delay(_time, cancellation.Token).ContinueWith(task =>
                    {
                        if (task.IsCanceled) return;
                        var v = prev;
                        prev = default;
                        destination.Next(v);
                    });
                },
                OnComplete = () =>
                {
                    if (prev is not null)
                    {
                        destination.Next(prev);
                    }

                    destination.Complete();
                    prev = default;
                }
            });
        });
    }
}

public static partial class FutureExtensions
{
    public static IFuture<T> Debounce<T>(this IFuture<T> future, TimeSpan time)
    {
        return future.Pipe(new Debounce<T>(time));
    }

    public static IFuture<T> Debounce<T>(this IFuture<T> future, int ms)
    {
        return future.Pipe(new Debounce<T>(TimeSpan.FromMilliseconds(ms)));
    }
}