namespace Futures.Operators;

public sealed class Debounce<T>(TimeSpan time) : IOperator<T>
{
    public Future<T> Invoke(Future<T> src)
    {
        CancellationTokenSource cancellation = new();

        return new Future<T>((value, destination) =>
        {
            cancellation.Cancel();
            cancellation = new();

            _ = Task.Delay(time, cancellation.Token).ContinueWith(task =>
            {
                if (task.IsCanceled) return;
                destination.Next(src.Next(value));
                destination.Complete();
            });
        });
    }
}

public sealed class Debounce<T, TOut>(TimeSpan time) : IOperator<T, TOut>
{
    public Future<T, TOut> Invoke(Future<T, TOut> src)
    {
        CancellationTokenSource cancellation = new();

        return new Future<T, TOut>((value, destination) =>
        {
            cancellation.Cancel();
            cancellation = new();

            _ = Task.Delay(time, cancellation.Token).ContinueWith(task =>
            {
                if (task.IsCanceled) return;
                destination.Next(src.Next(value));
                destination.Complete();
            });
        });
    }
}

public sealed class Debounce<T1, T2, TOut>(TimeSpan time) : IOperator<T1, T2, TOut>
{
    public Future<T1, T2, TOut> Invoke(Future<T1, T2, TOut> src)
    {
        CancellationTokenSource cancellation = new();

        return new Future<T1, T2, TOut>((a, b, destination) =>
        {
            cancellation.Cancel();
            cancellation = new();

            _ = Task.Delay(time, cancellation.Token).ContinueWith(task =>
            {
                if (task.IsCanceled) return;
                destination.Next(src.Next(a, b));
                destination.Complete();
            });
        });
    }
}

public static partial class FutureExtensions
{
    public static Future<T> Debounce<T>(this Future<T> future, TimeSpan time)
    {
        return future.Pipe(new Debounce<T>(time));
    }

    public static Future<T> Debounce<T>(this Future<T> future, int ms)
    {
        return future.Pipe(new Debounce<T>(TimeSpan.FromMilliseconds(ms)));
    }
}

public static partial class FutureExtensions
{
    public static Future<T, TOut> Debounce<T, TOut>(this Future<T, TOut> future, TimeSpan time)
    {
        return future.Pipe(new Debounce<T, TOut>(time));
    }

    public static Future<T, TOut> Debounce<T, TOut>(this Future<T, TOut> future, int ms)
    {
        return future.Pipe(new Debounce<T, TOut>(TimeSpan.FromMilliseconds(ms)));
    }
}

public static partial class FutureExtensions
{
    public static Future<T1, T2, TOut> Debounce<T1, T2, TOut>(this Future<T1, T2, TOut> future, TimeSpan time)
    {
        return future.Pipe(new Debounce<T1, T2, TOut>(time));
    }

    public static Future<T1, T2, TOut> Debounce<T1, T2, TOut>(this Future<T1, T2, TOut> future, int ms)
    {
        return future.Pipe(new Debounce<T1, T2, TOut>(TimeSpan.FromMilliseconds(ms)));
    }
}