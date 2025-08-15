namespace Futures;

public partial class Future<T>
{
    public Guid Id { get; } = Guid.NewGuid();
    public State State { get; protected set; } = State.NotStarted;
    public T? Last { get; protected set; }

    public bool IsComplete => IsSuccess || IsError || IsCancelled;
    public bool IsStarted => State == State.Started;
    public bool IsSuccess => State == State.Success;
    public bool IsError => State == State.Error;
    public bool IsCancelled => State == State.Cancelled;

    protected readonly List<(Guid, Subscriber<T, T>)> _subscribers = [];
    protected readonly TaskCompletionSource<T> _source;

    public Future(CancellationToken cancellation = default)
    {
        _source = new(cancellation);
        cancellation.Register(() =>
        {
            Cancel();
            UnSubscribe();
        });
    }

    public Future(Action<Future<T>> init, CancellationToken cancellation = default)
    {
        _source = new(cancellation);
        cancellation.Register(() =>
        {
            Cancel();
            UnSubscribe();
        });

        init(this);
    }

    internal IEnumerable<T> Next(T value)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Started;
        List<T> result = [];

        foreach (var (_, transformer) in _subscribers)
        {
            var items = transformer.OnNext(value);
            result.AddRange(items);
        }

        return result;
    }

    internal T? Complete()
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

        foreach (var (_, subscriber) in _subscribers)
        {
            subscriber.OnComplete();
        }

        return Last;
    }

    public virtual void Error(Exception ex)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Error;
        _source.TrySetException(ex);

        foreach (var (_, subscriber) in _subscribers)
        {
            subscriber.OnError(ex);
        }
    }

    public virtual void Cancel()
    {
        if (IsComplete) return;

        State = State.Cancelled;
        _source.TrySetCanceled();

        foreach (var (_, subscriber) in _subscribers)
        {
            subscriber.OnCancel();
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
        foreach (var (_, subscriber) in _subscribers)
        {
            subscriber.UnSubscribe();
        }
    }

    protected void UnSubscribe(Guid id)
    {
        var i = _subscribers.FindIndex(s => s.Item1 == id);

        if (i == -1)
        {
            return;
        }

        _subscribers.RemoveAt(i);
    }
}

public partial class Future<T, TOut>
{
    public Guid Id { get; } = Guid.NewGuid();
    public State State { get; protected set; } = State.NotStarted;
    public TOut? Last { get; protected set; }

    public bool IsComplete => IsSuccess || IsError || IsCancelled;
    public bool IsStarted => State == State.Started;
    public bool IsSuccess => State == State.Success;
    public bool IsError => State == State.Error;
    public bool IsCancelled => State == State.Cancelled;

    protected readonly List<(Guid, Subscriber<TOut, TOut>)> _listeners = [];
    protected readonly List<(Guid, Subscriber<T, TOut>)> _transformers = [];
    protected readonly TaskCompletionSource<TOut> _source;

    public Future(CancellationToken cancellation = default)
    {
        _source = new(cancellation);
        cancellation.Register(() =>
        {
            Cancel();
            UnSubscribe();
        });
    }

    public Future(Action<Future<T, TOut>> init, CancellationToken cancellation = default)
    {
        _source = new(cancellation);
        cancellation.Register(() =>
        {
            Cancel();
            UnSubscribe();
        });

        init(this);
    }

    internal IEnumerable<TOut> Next(T value)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Started;
        List<TOut> result = [];

        foreach (var (_, transformer) in _transformers)
        {
            var items = transformer.OnNext(value);

            foreach (var item in items)
            {
                foreach (var (_, listener) in _listeners)
                {
                    listener.OnNext(item);
                }
            }

            result.AddRange(items);
        }

        return result;
    }

    internal TOut? Complete()
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

        foreach (var (_, subscriber) in _transformers)
        {
            subscriber.OnComplete();
        }

        return Last;
    }

    public virtual void Error(Exception ex)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Error;
        _source.TrySetException(ex);

        foreach (var (_, subscriber) in _transformers)
        {
            subscriber.OnError(ex);
        }
    }

    public virtual void Cancel()
    {
        if (IsComplete) return;

        State = State.Cancelled;
        _source.TrySetCanceled();

        foreach (var (_, subscriber) in _transformers)
        {
            subscriber.OnCancel();
        }
    }

    public TOut Resolve()
    {
        return _source.Task.ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Task<TOut> AsTask()
    {
        return _source.Task;
    }

    public ValueTask<TOut> AsValueTask()
    {
        return new ValueTask<TOut>(_source.Task);
    }

    protected void UnSubscribe()
    {
        foreach (var (_, subscriber) in _transformers)
        {
            subscriber.UnSubscribe();
        }
    }

    protected void UnSubscribe(Guid id)
    {
        var i = _transformers.FindIndex(s => s.Item1 == id);

        if (i == -1)
        {
            i = _listeners.FindIndex(s => s.Item1 == id);

            if (i == -1) return;
        }

        _transformers.RemoveAt(i);
    }
}
