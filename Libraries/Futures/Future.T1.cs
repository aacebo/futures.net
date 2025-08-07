namespace Futures;

public class Future<T, TOut> : Future<TOut>
{
    private readonly Func<T, TOut> _resolver;

    public Future(Func<T, TOut> resolver, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolver = resolver;
    }

    public Future(Func<T, Task<TOut>> resolver, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolver = (input) => resolver(input).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Future(Func<T, Future<TOut>> resolver, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolver = (input) => resolver(input).Resolve();
    }

    public Future(Func<T, Task<Future<TOut>>> resolver, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolver = (input) => resolver(input).ConfigureAwait(false).GetAwaiter().GetResult().Resolve();
    }

    public Future(Action<T, Producer<TOut>> resolver, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolver = value =>
        {
            var future = new Future<TOut>(Token);
            resolver(value, new(future));
            return future.Resolve();
        };
    }

    public Future(Func<T, Producer<TOut>, Task> resolver, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolver = value =>
        {
            var future = new Future<TOut>(Token);
            resolver(value, new(future)).ConfigureAwait(false).GetAwaiter().GetResult();
            return future.Resolve();
        };
    }

    ~Future()
    {
        Dispose();
    }

    public TOut Next(T input)
    {
        return Next(_resolver(input));
    }

    public Task<TOut> NextAsync(T input)
    {
        return Task.FromResult(Next(input));
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

    public new Future<T, TNext> Pipe<TNext>(Func<TOut, TNext> next)
    {
        return new Future<T, TNext>(value => next(Next(value)), Token);
    }

    public new Future<T, TNext> Pipe<TNext>(Func<TOut, Task<TNext>> next)
    {
        return new Future<T, TNext>(value =>
        {
            return next(Next(value)).ConfigureAwait(false).GetAwaiter().GetResult();
        }, Token);
    }

    public new Future<T, TNext> Pipe<TNext>(Func<TOut, Future<TNext>> next)
    {
        return new Future<T, TNext>(value => next(Next(value)).Resolve(), Token);
    }

    public new Future<T, TNextOut> Pipe<TNext, TNextOut>(Func<TOut, Future<TNext, TNextOut>> next)
    {
        return new Future<T, TNextOut>(value => next(Next(value)).Resolve(), Token);
    }

    public new Future<T, TNext> Pipe<TNext>(Func<TOut, Task<Future<TNext>>> next)
    {
        return new Future<T, TNext>(value =>
        {
            return next(Next(value)).ConfigureAwait(false).GetAwaiter().GetResult().Resolve();
        }, Token);
    }

    public new Future<T, TNextOut> Pipe<TNext, TNextOut>(Func<TOut, Task<Future<TNext, TNextOut>>> next)
    {
        return new Future<T, TNextOut>(value =>
        {
            return next(Next(value)).ConfigureAwait(false).GetAwaiter().GetResult().Resolve();
        }, Token);
    }
}