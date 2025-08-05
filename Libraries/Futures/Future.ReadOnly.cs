using System.Collections;

namespace Futures;

/// <summary>
/// a future that is readonly, ie it can only be used
/// to read emitted data
/// </summary>
/// <typeparam name="TIn">the input type</typeparam>
/// <typeparam name="TOut">the output type</typeparam>
public interface IReadOnlyFuture<TIn, TOut> : IDisposable, IEnumerable<TOut>, IAsyncDisposable, IAsyncEnumerable<TOut>
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

    public ReadOnlyFuture<TIn, TNext> Pipe<TNext>(Func<TOut, TNext> next);
    public ReadOnlyFuture<TIn, TNext> Pipe<TNext>(Func<TOut, Task<TNext>> next);
    public ReadOnlyFuture<TIn, TNext> Pipe<TNext>(IFuture<TOut, TNext> next);
}

/// <summary>
/// a future that is readonly, ie it can only be used
/// to read emitted data
/// </summary>
/// <typeparam name="TIn">the input type</typeparam>
/// <typeparam name="TOut">the output type</typeparam>
public partial class ReadOnlyFuture<TIn, TOut> : IReadOnlyFuture<TIn, TOut>
{
    public Guid Id => _future.Id;
    public State State => _future.State;
    public TOut Value => _future.Value;
    public CancellationToken Token => _future.Token;
    public bool IsComplete => _future.IsComplete;
    public bool IsStarted => _future.IsStarted;
    public bool IsSuccess => _future.IsSuccess;
    public bool IsError => _future.IsError;
    public bool IsCancelled => _future.IsCancelled;

    protected readonly IFuture<TIn, TOut> _future;

    public ReadOnlyFuture(IFuture<TIn, TOut> future)
    {
        _future = future;
    }

    ~ReadOnlyFuture()
    {
        Dispose();
    }

    public void Dispose()
    {
        _future.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _future.DisposeAsync();
    }

    internal TOut Next(TIn input)
    {
        return _future.Next(input);
    }

    internal Task<TOut> NextAsync(TIn input)
    {
        return _future.NextAsync(input);
    }

    internal TOut Complete()
    {
        return _future.Complete();
    }

    internal Task<TOut> CompleteAsync()
    {
        return _future.CompleteAsync();
    }

    internal void Error(Exception ex)
    {
        _future.Error(ex);
    }

    internal Task ErrorAsync(Exception ex)
    {
        return _future.ErrorAsync(ex);
    }

    internal void Cancel()
    {
        _future.Cancel();
    }

    internal Task CancelAsync()
    {
        return _future.CancelAsync();
    }

    public TOut Resolve()
    {
        return _future.Resolve();
    }

    public Task<TOut> ResolveAsync()
    {
        return _future.ResolveAsync();
    }

    public Subscription Subscribe(Subscriber<TOut> subscriber)
    {
        return _future.Subscribe(subscriber);
    }

    public void UnSubscribe(Guid id)
    {
        _future.UnSubscribe(id);
    }

    public ReadOnlyFuture<TIn, TNext> Pipe<TNext>(Func<TOut, TNext> next)
    {
        return new ReadOnlyFuture<TIn, TNext>(_future.Pipe(next));
    }

    public ReadOnlyFuture<TIn, TNext> Pipe<TNext>(Func<TOut, Task<TNext>> next)
    {
        return new ReadOnlyFuture<TIn, TNext>(_future.Pipe(next));
    }

    public ReadOnlyFuture<TIn, TNext> Pipe<TNext>(IFuture<TOut, TNext> next)
    {
        return new ReadOnlyFuture<TIn, TNext>(_future.Pipe(next));
    }

    public IEnumerator<TOut> GetEnumerator()
    {
        return _future.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IAsyncEnumerator<TOut> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return _future.GetAsyncEnumerator(cancellationToken);
    }
}

/// <summary>
/// a future that is readonly, ie it can only be used
/// to read emitted data
/// </summary>
/// <typeparam name="T1">the first input type</typeparam>
/// <typeparam name="T2">the second type</typeparam>
/// <typeparam name="TOut">the output type</typeparam>
public interface IReadOnlyFuture<T1, T2, TOut> : IDisposable, IEnumerable<TOut>, IAsyncDisposable, IAsyncEnumerable<TOut>
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

    public IReadOnlyFuture<T1, T2, TNext> Pipe<TNext>(Func<TOut, TNext> next);
    public IReadOnlyFuture<T1, T2, TNext> Pipe<TNext>(Func<TOut, Task<TNext>> next);
    public IReadOnlyFuture<T1, T2, TNext> Pipe<TNext>(IFuture<TOut, TNext> next);
}

/// <summary>
/// a future that is readonly, ie it can only be used
/// to read emitted data
/// </summary>
/// <typeparam name="T1">the input type</typeparam>
/// <typeparam name="T2">the second input type</typeparam>
/// <typeparam name="TOut">the output type</typeparam>
public partial class ReadOnlyFuture<T1, T2, TOut> : IReadOnlyFuture<T1, T2, TOut>
{
    public Guid Id => _future.Id;
    public State State => _future.State;
    public TOut Value => _future.Value;
    public CancellationToken Token => _future.Token;
    public bool IsComplete => _future.IsComplete;
    public bool IsStarted => _future.IsStarted;
    public bool IsSuccess => _future.IsSuccess;
    public bool IsError => _future.IsError;
    public bool IsCancelled => _future.IsCancelled;

    protected readonly IFuture<T1, T2, TOut> _future;

    public ReadOnlyFuture(IFuture<T1, T2, TOut> future)
    {
        _future = future;
    }

    ~ReadOnlyFuture()
    {
        Dispose();
    }

    public void Dispose()
    {
        _future.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _future.DisposeAsync();
    }

    internal TOut Next(T1 a, T2 b)
    {
        return _future.Next(a, b);
    }

    internal Task<TOut> NextAsync(T1 a, T2 b)
    {
        return _future.NextAsync(a, b);
    }

    internal TOut Complete()
    {
        return _future.Complete();
    }

    internal Task<TOut> CompleteAsync()
    {
        return _future.CompleteAsync();
    }

    internal void Error(Exception ex)
    {
        _future.Error(ex);
    }

    internal Task ErrorAsync(Exception ex)
    {
        return _future.ErrorAsync(ex);
    }

    internal void Cancel()
    {
        _future.Cancel();
    }

    internal Task CancelAsync()
    {
        return _future.CancelAsync();
    }

    public TOut Resolve()
    {
        return _future.Resolve();
    }

    public Task<TOut> ResolveAsync()
    {
        return _future.ResolveAsync();
    }

    public Subscription Subscribe(Subscriber<TOut> subscriber)
    {
        return _future.Subscribe(subscriber);
    }

    public void UnSubscribe(Guid id)
    {
        _future.UnSubscribe(id);
    }

    public IReadOnlyFuture<T1, T2, TNext> Pipe<TNext>(Func<TOut, TNext> next)
    {
        return new ReadOnlyFuture<T1, T2, TNext>(_future.Pipe(next));
    }

    public IReadOnlyFuture<T1, T2, TNext> Pipe<TNext>(Func<TOut, Task<TNext>> next)
    {
        return new ReadOnlyFuture<T1, T2, TNext>(_future.Pipe(next));
    }

    public IReadOnlyFuture<T1, T2, TNext> Pipe<TNext>(IFuture<TOut, TNext> next)
    {
        return new ReadOnlyFuture<T1, T2, TNext>(_future.Pipe(next));
    }

    public IEnumerator<TOut> GetEnumerator()
    {
        return _future.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IAsyncEnumerator<TOut> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return _future.GetAsyncEnumerator(cancellationToken);
    }
}