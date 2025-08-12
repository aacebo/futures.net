namespace Futures;

/// <summary>
/// an async value that can be resolved and subscribed to
/// </summary>
/// <typeparam name="T">the type being consumed/resolved/emitted</typeparam>
public interface IFuture<T> : IDisposable, IAsyncDisposable, IEnumerable<T>, IAsyncEnumerable<T>, IProducer<T>, IConsumer<T>
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

    IFuture<TNext> Pipe<TNext>(Func<T, TNext> next);
    IFuture<TNext> Pipe<TNext>(Func<T, Task<TNext>> next);
    IFuture<TNext> Pipe<TNext>(Func<T, IFuture<TNext>> next);
    IFuture<T> Pipe(IFuture<T> next);
    IFuture<TNext> Pipe<TNext>(IFuture<T, TNext> next);

    public delegate void Resolver(T value, IConsumer<T> consumer);
}

/// <summary>
/// an async value that can be resolved and subscribed to
/// </summary>
/// <typeparam name="T">the type being consumed</typeparam>
/// <typeparam name="TOut">the type being resolved/emitted</typeparam>
public interface IFuture<T, TOut> : IDisposable, IAsyncDisposable, IEnumerable<T>, IAsyncEnumerable<T>, IProducer<TOut>, IConsumer<T, TOut>
{
    Guid Id { get; }
    State State { get; }
    TOut Value { get; }
    CancellationToken Token { get; }

    public bool IsComplete { get; }
    public bool IsStarted { get; }
    public bool IsSuccess { get; }
    public bool IsError { get; }
    public bool IsCancelled { get; }

    TOut Resolve();
    Task<TOut> AsTask();
    ValueTask<TOut> AsValueTask();

    IFuture<TNext> Pipe<TNext>(Func<TOut, TNext> next);
    IFuture<TNext> Pipe<TNext>(Func<TOut, Task<TNext>> next);
    IFuture<TNext> Pipe<TNext>(Func<TOut, IFuture<TNext>> next);
    IFuture<T> Pipe(IFuture<TOut> next);
    IFuture<TNext> Pipe<TNext>(IFuture<TOut, TNext> next);

    public delegate void Resolver(T value, IConsumer<TOut> consumer);
}