namespace Futures;

/// <summary>
/// a future that is readonly, ie it can only be used
/// to read emitted data
/// </summary>
/// <typeparam name="TOut">the output type</typeparam>
public interface IReadOnlyFuture<TOut> : IDisposable, IEnumerable<TOut>, IAsyncDisposable, IAsyncEnumerable<TOut>
{
    public Guid Id { get; }
    public State State { get; }
    public TOut Value { get; }
    public CancellationToken Token { get; }

    public bool IsComplete { get; }
    public bool IsStarted { get; }
    public bool IsSuccess { get; }
    public bool IsError { get; }
    public bool IsCancelled { get; }

    public TOut Resolve();
    public Task<TOut> ResolveAsync();

    public Subscription Subscribe(Subscriber<TOut> subscriber);
    public void UnSubscribe(Guid id);

    public IReadOnlyFuture<TNext> Pipe<TNext>(Func<TOut, TNext> next);
    public IReadOnlyFuture<TNext> Pipe<TNext>(Func<TOut, Task<TNext>> next);
    public IReadOnlyFuture<TNext> Pipe<TNext>(IFuture<TOut, TNext> next);
}

/// <summary>
/// a future that is readonly, ie it can only be used
/// to read emitted data
/// </summary>
/// <typeparam name="TIn">the input type</typeparam>
/// <typeparam name="TOut">the output type</typeparam>
public partial class ReadOnlyFuture<TIn, TOut> : FutureBase<TOut>, IReadOnlyFuture<TOut>
{
    protected readonly IFuture<TIn, TOut> _future;

    public ReadOnlyFuture(IFuture<TIn, TOut> future, CancellationToken cancellation = default) : base(cancellation)
    {
        _future = future;
    }

    public IReadOnlyFuture<TNext> Pipe<TNext>(Func<TOut, TNext> next)
    {
        return new ReadOnlyFuture<TIn, TNext>(_future.Pipe(next));
    }

    public IReadOnlyFuture<TNext> Pipe<TNext>(Func<TOut, Task<TNext>> next)
    {
        return new ReadOnlyFuture<TIn, TNext>(_future.Pipe(next));
    }

    public IReadOnlyFuture<TNext> Pipe<TNext>(IFuture<TOut, TNext> next)
    {
        return new ReadOnlyFuture<TIn, TNext>(_future.Pipe(next));
    }
}

/// <summary>
/// a future that is readonly, ie it can only be used
/// to read emitted data
/// </summary>
/// <typeparam name="T1">the input type</typeparam>
/// <typeparam name="T2">the second input type</typeparam>
/// <typeparam name="TOut">the output type</typeparam>
public partial class ReadOnlyFuture<T1, T2, TOut> : FutureBase<TOut>, IReadOnlyFuture<TOut>
{
    protected readonly IFuture<T1, T2, TOut> _future;

    public ReadOnlyFuture(IFuture<T1, T2, TOut> future, CancellationToken cancellation = default) : base(cancellation)
    {
        _future = future;
    }

    public IReadOnlyFuture<TNext> Pipe<TNext>(Func<TOut, TNext> next)
    {
        return new ReadOnlyFuture<T1, T2, TNext>(_future.Pipe(next));
    }

    public IReadOnlyFuture<TNext> Pipe<TNext>(Func<TOut, Task<TNext>> next)
    {
        return new ReadOnlyFuture<T1, T2, TNext>(_future.Pipe(next));
    }

    public IReadOnlyFuture<TNext> Pipe<TNext>(IFuture<TOut, TNext> next)
    {
        return new ReadOnlyFuture<T1, T2, TNext>(_future.Pipe(next));
    }
}