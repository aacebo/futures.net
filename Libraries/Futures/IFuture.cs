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

    public bool IsComplete => IsSuccess || IsError || IsCancelled;
    public bool IsStarted => State == State.Started;
    public bool IsSuccess => State == State.Success;
    public bool IsError => State == State.Error;
    public bool IsCancelled => State == State.Cancelled;

    T Resolve();
    Task<T> AsTask();
    ValueTask<T> AsValueTask();
    Subscription Subscribe(IConsumer<T> consumer);

    IFuture<TNext> Pipe<TNext>(Func<T, TNext> next);
    IFuture<TNext> Pipe<TNext>(Func<T, Task<TNext>> next);
    IFuture<T> Pipe(ITopic<T> topic);
    IFuture<TNext> Pipe<TNext>(IStream<T, TNext> stream);
}