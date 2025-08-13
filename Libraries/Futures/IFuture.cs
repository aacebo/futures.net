namespace Futures;

/// <summary>
/// an async value that can be resolved and subscribed to
/// </summary>
/// <typeparam name="T">the type being consumed/resolved/emitted</typeparam>
public interface IFuture<T> : IDisposable, IAsyncDisposable, IEnumerable<T>, IAsyncEnumerable<T>, IProducer<T>
{
    Guid Id { get; }
    State State { get; }

    public bool IsComplete { get; }
    public bool IsStarted { get; }
    public bool IsSuccess { get; }
    public bool IsError { get; }
    public bool IsCancelled { get; }

    T Resolve();
    Task<T> AsTask();
    ValueTask<T> AsValueTask();

    IFuture<T> Pipe(IOperator<T> @operator);
    IFuture<TNext> Pipe<TNext>(IOperator<T, TNext> @operator);
}