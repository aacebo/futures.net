namespace Futures;

public partial class Future<T> : IFuture<T>
{
    public IFuture<TNext> Pipe<TNext>(Func<T, TNext> next)
    {
        var future = new Future<TNext>(Token);

        Subscribe(new Consumer<T>()
        {
            OnNext = v => future.Next(next(v)),
            OnComplete = future.Complete,
            OnError = future.Error,
            OnCancel = future.Cancel
        });

        return future;
    }

    public IFuture<TNext> Pipe<TNext>(Func<T, Task<TNext>> next)
    {
        return Pipe(v => next(v).ConfigureAwait(false).GetAwaiter().GetResult());
    }

    public IFuture<TNext> Pipe<TNext>(Func<T, IFuture<TNext>> next)
    {
        return Pipe(v => next(v).Resolve());
    }

    public IFuture<T> Pipe(IFuture<T> next)
    {
        return Pipe(v =>
        {
            next.Next(v);
            return v;
        });
    }

    public IFuture<TNext> Pipe<TNext>(IFuture<T, TNext> next)
    {
        return Pipe(next.Next);
    }
}