namespace Futures;

public class Future<T1, T2, TOut> : Future<TOut>
{
    private readonly Func<T1, T2, TOut> _resolver;

    public Future(Func<T1, T2, TOut> resolver, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolver = resolver;
    }

    public Future(Func<T1, T2, Task<TOut>> resolver, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolver = (a, b) => resolver(a, b).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Future(Action<T1, T2, Producer<TOut>> resolver, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolver = (a, b) =>
        {
            var future = new Future<TOut>(Token);
            resolver(a, b, new(future));
            return future.Resolve();
        };
    }

    public Future(Func<T1, T2, Producer<TOut>, Task> resolver, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolver = (a, b) =>
        {
            var future = new Future<TOut>(Token);
            resolver(a, b, new(future)).ConfigureAwait(false).GetAwaiter().GetResult();
            return future.Resolve();
        };
    }

    ~Future()
    {
        Dispose();
    }

    public TOut Next(T1 a, T2 b)
    {
        return Next(_resolver(a, b));
    }

    public Task<TOut> NextAsync(T1 a, T2 b)
    {
        return Task.FromResult(Next(a, b));
    }

    public new TOut Complete()
    {
        return base.Complete();
    }

    public new Task<TOut> CompleteAsync()
    {
        return base.CompleteAsync();
    }

    public new void Error(Exception error)
    {
        base.Error(error);
    }

    public new Task ErrorAsync(Exception error)
    {
        return base.ErrorAsync(error);
    }

    public new void Cancel()
    {
        base.Cancel();
    }

    public new Task CancelAsync()
    {
        return base.CancelAsync();
    }

    public new Future<T1, T2, TNext> Pipe<TNext>(Func<TOut, TNext> next)
    {
        return new Future<T1, T2, TNext>((a, b) => next(Next(a, b)), Token);
    }

    public new Future<T1, T2, TNext> Pipe<TNext>(Func<TOut, Task<TNext>> next)
    {
        return Pipe(v => next(v).ConfigureAwait(false).GetAwaiter().GetResult());
    }

    public new Future<T1, T2, TNext> Pipe<TNext>(Action<TOut, Producer<TNext>> next)
    {
        return Pipe(v =>
        {
            var future = new Future<TNext>(Token);
            next(v, new(future));
            return future.Resolve();
        });
    }

    public new Future<T1, T2, TNext> Pipe<TNext>(Func<TOut, Producer<TNext>, Task> next)
    {
        return Pipe(v =>
        {
            var future = new Future<TNext>(Token);
            next(v, new(future));
            return future.Resolve();
        });
    }

    public new Future<T1, T2, TNext> Pipe<TNext>(Func<TOut, Future<TNext>> next)
    {
        return Pipe(v => next(v).Resolve());
    }

    public new Future<T1, T2, TNextOut> Pipe<TNext, TNextOut>(Func<TOut, Future<TNext, TNextOut>> next)
    {
        return Pipe(v => next(v).Resolve());
    }

    public new Future<T1, T2, TNext> Pipe<TNext>(Func<TOut, Task<Future<TNext>>> next)
    {
        return Pipe(v => next(v).ConfigureAwait(false).GetAwaiter().GetResult().Resolve());
    }

    public new Future<T1, T2, TNextOut> Pipe<TNext, TNextOut>(Func<TOut, Task<Future<TNext, TNextOut>>> next)
    {
        return Pipe(v => next(v).ConfigureAwait(false).GetAwaiter().GetResult().Resolve());
    }
}