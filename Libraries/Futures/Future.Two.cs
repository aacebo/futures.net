namespace Futures;

/// <summary>
/// a future that transforms some input into a different shaped output
/// </summary>
/// <typeparam name="TIn">the input type</typeparam>
/// <typeparam name="TOut">the output type</typeparam>
public interface IFuture<TIn, TOut> : IDisposable, IEnumerable<TOut>, IAsyncDisposable, IAsyncEnumerable<TOut>
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

    public Subscription Subscribe(Subscriber<TOut> subscriber);
    public void UnSubscribe(Guid id);

    public IFuture<TIn, TNext> Pipe<TNext>(Func<TOut, TNext> next);
    public IFuture<TIn, TNext> Pipe<TNext>(Func<TOut, Task<TNext>> next);
    public IFuture<TIn, TNext> Pipe<TNext>(IFuture<TOut, TNext> next);
}

/// <summary>
/// a future that transforms some input into a different shaped output
/// </summary>
/// <typeparam name="TIn">the input type</typeparam>
/// <typeparam name="TOut">the output type</typeparam>
public partial class Future<TIn, TOut> : FutureBase<TOut>, IFuture<TIn, TOut>
{
    protected Func<TIn, TOut> Resolver;

    public Future(Func<TIn, TOut> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        Resolver = resolve;
    }

    public Future(Func<TIn, Task<TOut>> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        Resolver = (input) => resolve(input).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Future(Func<TIn, IFuture<TOut>> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        Resolver = (input) => resolve(input).Resolve();
    }

    public Future(Func<TIn, IFuture<TIn, TOut>> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        Resolver = (input) => resolve(input).Resolve();
    }

    public Future(Action<TIn, Subscriber<TOut>> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        Resolver = (input) =>
        {
            var future = new Future<TOut>();

            resolve(input, new()
            {
                OnNext = value => future.Next(value),
                OnComplete = () => future.Complete(),
                OnError = ex => future.Error(ex),
                OnCancel = () => future.Cancel(),
            });

            return future.Resolve();
        };
    }

    public Future(Func<TIn, Subscriber<TOut>, Task> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        Resolver = (input) =>
        {
            var future = new Future<TOut>();

            resolve(input, new()
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

    public virtual TOut Next(TIn value)
    {
        if (IsComplete)
        {
            return Value;
        }

        State = State.Started;
        var output = Resolver(value);
        Value = output;

        foreach (var (_, subscriber) in Subscribers)
        {
            subscriber.Next(output);
        }

        return output;
    }

    public virtual Task<TOut> NextAsync(TIn value)
    {
        return Task.FromResult(Next(value));
    }

    public IFuture<TIn, TNext> Pipe<TNext>(Func<TOut, TNext> next)
    {
        return new Future<TIn, TNext>(value =>
        {
            return next(Next(value));
        });
    }

    public IFuture<TIn, TNext> Pipe<TNext>(Func<TOut, Task<TNext>> next)
    {
        return new Future<TIn, TNext>(value =>
        {
            return next(Next(value));
        });
    }

    public IFuture<TIn, TNext> Pipe<TNext>(IFuture<TOut, TNext> next)
    {
        return new Future<TIn, TNext>(value =>
        {
            return next.Next(Next(value));
        });
    }

    public static implicit operator ReadOnlyFuture<TIn, TOut>(Future<TIn, TOut> future) => new(future);
}