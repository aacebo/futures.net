namespace Futures;

public interface IFuture<TIn, TOut> : IDisposable, IAsyncEnumerable<TOut>
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

    public TOut Next(TIn input);
    public Task<TOut> NextAsync(TIn input);
    public TOut Complete();
    public Task<TOut> CompleteAsync();
    public void Error(Exception ex);
    public Task ErrorAsync(Exception ex);
    public void Cancel();
    public Task CancelAsync();

    public TOut Resolve();
    public Task<TOut> ResolveAsync();

    public IFuture<TIn, TNext> Pipe<TNext>(Func<TOut, TNext> next);
    public IFuture<TIn, TNext> Pipe<TNext>(Func<TOut, Task<TNext>> next);
    public IFuture<TIn, TNext> Pipe<TNext>(IFuture<TOut, TNext> next);

    public Subscription Subscribe(Subscriber<TOut> subscriber);
    public void UnSubscribe(Guid id);
}

public interface IFuture<T> : IFuture<T, T>;

public enum State
{
    NotStarted,
    Started,
    Success,
    Error,
    Cancelled,
}