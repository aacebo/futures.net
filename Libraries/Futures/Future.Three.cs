namespace Futures;

/// <summary>
/// a future that transforms some input into a different shaped output
/// </summary>
/// <typeparam name="T1">the first input type</typeparam>
/// <typeparam name="T2">the second input type</typeparam>
/// <typeparam name="TOut">the output type</typeparam>
public interface IFuture<T1, T2, TOut> : IDisposable, IEnumerable<TOut>, IAsyncDisposable, IAsyncEnumerable<TOut>
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

    public TOut Next(T1 a, T2 b);
    public Task<TOut> NextAsync(T1 a, T2 b);
    public TOut Complete();
    public Task<TOut> CompleteAsync();
    public void Error(Exception ex);
    public Task ErrorAsync(Exception ex);
    public void Cancel();
    public Task CancelAsync();

    public TOut Resolve();
    public Task<TOut> ResolveAsync();

    public Subscription Subscribe(Subscriber<TOut> subscriber);
    public void UnSubscribe(Guid id);

    public IFuture<T1, T2, TNext> Pipe<TNext>(Func<TOut, TNext> next);
    public IFuture<T1, T2, TNext> Pipe<TNext>(Func<TOut, Task<TNext>> next);
    public IFuture<T1, T2, TNext> Pipe<TNext>(IFuture<TOut, TNext> next);
}

/// <summary>
/// a future that transforms some input into a different shaped output
/// </summary>
/// <typeparam name="T1">the first input type</typeparam>
/// <typeparam name="T2">the second input type</typeparam>
/// <typeparam name="TOut">the output type</typeparam>
public partial class Future<T1, T2, TOut> : FutureBase<TOut>, IFuture<T1, T2, TOut>
{
    protected Func<T1, T2, TOut> Resolver;

    public Future(Func<T1, T2, TOut> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        Resolver = resolve;
    }

    public Future(Func<T1, T2, Task<TOut>> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        Resolver = (a, b) => resolve(a, b).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Future(Func<T1, T2, IFuture<TOut>> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        Resolver = (a, b) => resolve(a, b).Resolve();
    }

    public Future(Func<T1, T2, IFuture<T1, T2, TOut>> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        Resolver = (a, b) => resolve(a, b).Resolve();
    }

    public Future(Action<T1, T2, Subscriber<TOut>> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        Resolver = (a, b) =>
        {
            var future = new Future<TOut>();

            resolve(a, b, new()
            {
                OnNext = value => future.Next(value),
                OnComplete = () => future.Complete(),
                OnError = ex => future.Error(ex),
                OnCancel = () => future.Cancel(),
            });

            return future.Resolve();
        };
    }

    public Future(Func<T1, T2, Subscriber<TOut>, Task> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        Resolver = (a, b) =>
        {
            var future = new Future<TOut>();

            resolve(a, b, new()
            {
                OnNext = value => future.Next(value),
                OnComplete = () => future.Complete(),
                OnError = ex => future.Error(ex),
                OnCancel = () => future.Cancel(),
            }).GetAwaiter().GetResult();

            return future.Resolve();
        };
    }

    ~Future()
    {
        Dispose();
    }

    public virtual TOut Next(T1 a, T2 b)
    {
        if (IsComplete)
        {
            return Value;
        }

        State = State.Started;
        var output = Resolver(a, b);
        Value = output;

        foreach (var (_, subscriber) in Subscribers)
        {
            subscriber.Next(output);
        }

        return output;
    }

    public virtual Task<TOut> NextAsync(T1 a, T2 b)
    {
        return Task.FromResult(Next(a, b));
    }

    public IFuture<T1, T2, TNext> Pipe<TNext>(Func<TOut, TNext> next)
    {
        return new Future<T1, T2, TNext>((a, b) =>
        {
            return next(Next(a, b));
        });
    }

    public IFuture<T1, T2, TNext> Pipe<TNext>(Func<TOut, Task<TNext>> next)
    {
        return new Future<T1, T2, TNext>((a, b) =>
        {
            return next(Next(a, b));
        });
    }

    public IFuture<T1, T2, TNext> Pipe<TNext>(IFuture<TOut, TNext> next)
    {
        return new Future<T1, T2, TNext>((a, b) =>
        {
            return next.Next(Next(a, b));
        });
    }

    public static implicit operator ReadOnlyFuture<T1, T2, TOut>(Future<T1, T2, TOut> future) => new(future);
}