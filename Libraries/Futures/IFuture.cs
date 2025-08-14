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
    TFuture As<TFuture>() where TFuture : IFuture<T>;

    IFuture<TNext> Pipe<TNext>(IOperator<T, TNext> @operator);
}

public interface IFuture<T, TOut> : IFuture<TOut>
{
    TOut Next(T value);
    TOut Complete();
    void Error(Exception ex);
    void Cancel();

    IFuture<T, TNext> Pipe<TNext>(IOperator<T, TOut, TNext> @operator);
}

public interface IFuture<T1, T2, TOut> : IFuture<TOut>
{
    TOut Next(T1 a, T2 b);
    TOut Complete();
    void Error(Exception ex);
    void Cancel();

    IFuture<T1, T2, TNext> Pipe<TNext>(IOperator<T1, T2, TOut, TNext> @operator);
}