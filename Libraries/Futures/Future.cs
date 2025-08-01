namespace Futures;

public delegate void ResolveCallback<TIn, TOut>(TIn value, Subscriber<TOut> subscriber);

public partial class Future<T> : Future<T, T>, IFuture<T>, IFuture<T, T>
{
    public Future(CancellationToken cancellation = default) : base(value => value, cancellation)
    {

    }

    public Future(T value, CancellationToken cancellation = default) : base(value => value, cancellation)
    {
        Next(value);
    }

    public Future(Func<T, T> resolve, CancellationToken cancellation = default) : base(resolve, cancellation)
    {

    }

    public static Future<T> From(T value)
    {
        var future = new Future<T>(value);
        future.Complete();
        return future;
    }

    public static Future<T> From(Exception ex)
    {
        var future = new Future<T>();
        future.Error(ex);
        return future;
    }
}

public partial class Future<TIn, TOut> : IFuture<TIn, TOut>
{
    public Guid Id { get; } = Guid.NewGuid();
    public State State { get; private set; } = State.NotStarted;
    public TOut Value { get; private set; } = default!;
    public CancellationToken Token { get; private set; }

    public bool IsComplete => IsSuccess || IsError || IsCancelled;
    public bool IsStarted => State == State.Started;
    public bool IsSuccess => State == State.Success;
    public bool IsError => State == State.Error;
    public bool IsCancelled => State == State.Cancelled;

    protected Func<TIn, TOut> Resolver;
    protected readonly TaskCompletionSource<TOut> Source;
    protected readonly List<(Guid, Subscriber<TOut>)> Subscribers = [];

    public Future(CancellationToken cancellation = default)
    {
        Resolver = _ => throw new NotImplementedException();
        Token = cancellation;
        Source = new(cancellation);
    }

    public Future(Func<TIn, TOut> resolve, CancellationToken cancellation = default)
    {
        Resolver = resolve;
        Token = cancellation;
        Source = new(cancellation);
    }

    public Future(Func<TIn, Task<TOut>> resolve, CancellationToken cancellation = default)
    {
        Resolver = (input) => resolve(input).ConfigureAwait(false).GetAwaiter().GetResult();
        Token = cancellation;
        Source = new(cancellation);
    }

    public Future(Func<TIn, IFuture<TOut>> resolve, CancellationToken cancellation = default)
    {
        Resolver = (input) => resolve(input).Resolve();
        Token = cancellation;
        Source = new(cancellation);
    }

    public Future(Func<TIn, IFuture<TIn, TOut>> resolve, CancellationToken cancellation = default)
    {
        Resolver = (input) => resolve(input).Resolve();
        Token = cancellation;
        Source = new(cancellation);
    }

    public Future(ResolveCallback<TIn, TOut> resolve, CancellationToken cancellation = default)
    {
        Token = cancellation;
        Source = new(cancellation);
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

    ~Future()
    {
        Dispose();
    }

    public virtual void Dispose()
    {
        foreach (var (_, subscriber) in Subscribers)
        {
            subscriber.Dispose();
        }

        if (!IsComplete)
        {
            Cancel();
        }

        Source.Task.Dispose();
        GC.SuppressFinalize(this);
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

    public virtual TOut Complete()
    {
        if (Value is null)
        {
            throw new InvalidOperationException("attempted to complete a future with no data");
        }

        if (IsComplete)
        {
            return Value;
        }

        State = State.Success;
        Source.TrySetResult(Value);

        foreach (var (_, subscriber) in Subscribers)
        {
            subscriber.Complete();
        }

        return Value;
    }

    public virtual Task<TOut> CompleteAsync()
    {
        return Task.FromResult(Complete());
    }

    public virtual void Error(Exception ex)
    {
        if (IsComplete)
        {
            return;
        }

        State = State.Error;
        Source.TrySetException(ex);

        foreach (var (_, subscriber) in Subscribers)
        {
            subscriber.Error(ex);
        }
    }

    public virtual Task ErrorAsync(Exception ex)
    {
        Error(ex);
        return Task.CompletedTask;
    }

    public virtual void Cancel()
    {
        if (IsComplete)
        {
            return;
        }

        State = State.Cancelled;
        Source.TrySetCanceled();

        foreach (var (_, subscriber) in Subscribers)
        {
            subscriber.Cancel();
        }
    }

    public virtual Task CancelAsync()
    {
        Cancel();
        return Task.CompletedTask;
    }

    public virtual TOut Resolve()
    {
        return Source.Task.ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public virtual Task<TOut> ResolveAsync()
    {
        return Source.Task;
    }

    public virtual Subscription Subscribe(Subscriber<TOut> subscriber)
    {
        var id = Guid.NewGuid();
        var subscription = new Subscription(() => UnSubscribe(id));
        Subscribers.Add((id, subscriber));
        return subscription;
    }

    public virtual void UnSubscribe(Guid id)
    {
        var i = Subscribers.FindIndex(s => s.Item1 == id);

        if (i == -1)
        {
            return;
        }

        Subscribers.RemoveAt(i);
    }
}