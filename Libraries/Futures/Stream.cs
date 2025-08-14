namespace Futures;

public partial class Stream<T>
{
    public Guid Id { get; } = Guid.NewGuid();
    public State State { get; protected set; } = State.NotStarted;

    public bool IsComplete => IsSuccess || IsError || IsCancelled;
    public bool IsStarted => State == State.Started;
    public bool IsSuccess => State == State.Success;
    public bool IsError => State == State.Error;
    public bool IsCancelled => State == State.Cancelled;

    protected T? Last { get; set; }
    protected readonly List<(Guid, Subscriber<T>)> _consumers = [];
    protected SubscribeFn _subscribe;
    protected readonly TaskCompletionSource<T> _source;

    public Stream(CancellationToken cancellation = default)
    {
        _source = new(cancellation);
        _subscribe = (_) => () => { };
        cancellation.Register(() =>
        {
            Cancel();
            UnSubscribe();
        });
    }

    public Stream(Fn<IConsumer<T>> subscribe, CancellationToken cancellation = default)
    {
        _source = new(cancellation);
        _subscribe = (consumer) =>
        {
            subscribe.Invoke(consumer);
            return () => { };
        };

        cancellation.Register(() =>
        {
            Cancel();
            UnSubscribe();
        });
    }

    public Stream(Fn<IConsumer<T>, Action> subscribe, CancellationToken cancellation = default)
    {
        _source = new(cancellation);
        _subscribe = subscribe.Invoke;
        cancellation.Register(() =>
        {
            Cancel();
            UnSubscribe();
        });
    }

    internal void Push(T value)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Started;
        Last = value;

        foreach (var (_, subscriber) in _consumers)
        {
            subscriber.Next(value);
        }
    }

    internal void Done()
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Success;

        if (Last is not null)
        {
            _source.TrySetResult(Last);
        }

        foreach (var (_, subscriber) in _consumers)
        {
            subscriber.Complete();
        }
    }

    public virtual void Error(Exception ex)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Error;
        _source.TrySetException(ex);

        foreach (var (_, subscriber) in _consumers)
        {
            subscriber.Error(ex);
        }
    }

    public virtual void Cancel()
    {
        if (IsComplete) return;

        State = State.Cancelled;
        _source.TrySetCanceled();

        foreach (var (_, subscriber) in _consumers)
        {
            subscriber.Cancel();
        }
    }

    public T Resolve()
    {
        return _source.Task.ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Task<T> AsTask()
    {
        return _source.Task;
    }

    public ValueTask<T> AsValueTask()
    {
        return new ValueTask<T>(_source.Task);
    }

    protected void UnSubscribe()
    {
        foreach (var (_, subscriber) in _consumers)
        {
            subscriber.UnSubscribe();
        }
    }

    protected void UnSubscribe(Guid id)
    {
        var i = _consumers.FindIndex(s => s.Item1 == id);

        if (i == -1)
        {
            return;
        }

        _consumers.RemoveAt(i);
    }

    public delegate Action SubscribeFn(IConsumer<T> consumer);
}
