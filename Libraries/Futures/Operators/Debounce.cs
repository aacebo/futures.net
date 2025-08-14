namespace Futures.Operators;

public sealed class Debounce<T>(TimeSpan time) : IOperator<T, T>
{
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

                    _ = Task.Delay(time, cancellation.Token).ContinueWith(task =>
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

public sealed class Debounce<T, TOut>(TimeSpan time) : IOperator<T, TOut, TOut>
{
    public IFuture<T, TOut> Invoke(IFuture<T, TOut> source)
    {

        TOut? prev = default;
        CancellationTokenSource cancellation = new();

        return new Future<T, TOut>((value, destination) =>
        {
            prev = source.Next(value);
            cancellation.Cancel();
            cancellation = new();

            _ = Task.Delay(time, cancellation.Token).ContinueWith(task =>
            {
                if (task.IsCanceled) return;
                var v = prev;
                prev = default;
                destination.Next(v);
                destination.Complete();
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

public static partial class FutureExtensions
{
    public static IFuture<T, TOut> Debounce<T, TOut>(this IFuture<T, TOut> future, TimeSpan time)
    {
        return future.Pipe(new Debounce<T, TOut>(time));
    }

    public static IFuture<T, TOut> Debounce<T, TOut>(this IFuture<T, TOut> future, int ms)
    {
        return future.Pipe(new Debounce<T, TOut>(TimeSpan.FromMilliseconds(ms)));
    }
}