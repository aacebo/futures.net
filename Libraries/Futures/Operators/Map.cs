namespace Futures.Operators;

public sealed class Map<T, TOut> : IOperator<T, TOut>
{
    private readonly Func<T, int, TOut> _next;

    public Map(Func<T, int, TOut> next)
    {
        _next = next;
    }

    public Map(Func<T, int, Task<TOut>> next)
    {
        _next = (value, i) => next(value, i).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public IFuture<TOut> Invoke(IFuture<T> source)
    {
        return new Future<TOut>((destination, consumer) =>
        {
            var i = 0;

            source.Subscribe(new Consumer<T>()
            {
                OnNext = value => destination.Next(_next(value, i++)),
                OnComplete = () => destination.Complete(),
                OnError = destination.Error,
                OnCancel = destination.Cancel
            });
        }, source.Token);
    }
}

public static partial class FutureExtensions
{
    public static IFuture<TNext> Map<T, TNext>(this IFuture<T> future, Func<T, int, TNext> next)
    {
        return future.Pipe(new Map<T, TNext>(next));
    }

    public static IFuture<TNext> Map<T, TNext>(this IFuture<T> future, Func<T, int, Task<TNext>> next)
    {
        return future.Pipe(new Map<T, TNext>(next));
    }

    public static IFuture<T, TNext> Map<T, TOut, TNext>(this IFuture<T, TOut> future, Func<TOut, int, TNext> next)
    {
        return future.Pipe(new Map<TOut, TNext>(next));
    }

    public static IFuture<T, TNext> Map<T, TOut, TNext>(this IFuture<T, TOut> future, Func<TOut, int, Task<TNext>> next)
    {
        return future.Pipe(new Map<TOut, TNext>(next));
    }
}