namespace Futures;

/// <summary>
/// an async value that can be resolved and subscribed to
/// </summary>
/// <typeparam name="T">the type being resolved/emitted</typeparam>
public interface IFuture<T> : IDisposable, IAsyncDisposable, IEnumerable<T>, IAsyncEnumerable<T>
{
    Guid Id { get; }
    State State { get; }
    T Value { get; }
    CancellationToken Token { get; }

    public bool IsComplete { get; }
    public bool IsStarted { get; }
    public bool IsSuccess { get; }
    public bool IsError { get; }
    public bool IsCancelled { get; }

    T Resolve();
    Task<T> AsTask();
    ValueTask<T> AsValueTask();
    Subscription Subscribe(IConsumer<T> consumer);

    IFuture<TNext> Pipe<TNext>(Func<T, TNext> next);
    IFuture<TNext> Pipe<TNext>(Func<T, Task<TNext>> next);
    IFuture<TNext> Pipe<TNext>(Func<T, IFuture<TNext>> next);
    IFuture<T> Pipe(ITopic<T> topic);
    IFuture<T> Pipe(ITopic<Task<T>> topic);
    IFuture<TNext> Pipe<TNext>(IStream<T, TNext> stream);
    IFuture<TNext> Pipe<TNext>(IStream<T, Task<TNext>> stream);
}