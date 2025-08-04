using System.Collections;

namespace Futures;

public abstract class FutureBase<TOut> : IEnumerable<TOut>, IAsyncEnumerable<TOut>
{
    public Guid Id { get; } = Guid.NewGuid();
    public State State { get; protected set; } = State.NotStarted;
    public TOut Value { get; protected set; } = default!;
    public CancellationToken Token { get; protected set; }

    public bool IsComplete => IsSuccess || IsError || IsCancelled;
    public bool IsStarted => State == State.Started;
    public bool IsSuccess => State == State.Success;
    public bool IsError => State == State.Error;
    public bool IsCancelled => State == State.Cancelled;

    protected readonly TaskCompletionSource<TOut> Source;
    protected readonly List<(Guid, Subscriber<TOut>)> Subscribers = [];

    public FutureBase(CancellationToken cancellation = default)
    {
        Token = cancellation;
        Source = new(cancellation);
    }

    ~FutureBase()
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

    public virtual ValueTask DisposeAsync()
    {
        Dispose();
        return default;
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

    public IEnumerator<TOut> GetEnumerator()
    {
        return new FutureEnumerator<TOut>(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IAsyncEnumerator<TOut> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new FutureEnumerator<TOut>(this);
    }
}