namespace Futures;

public abstract class FutureBase<T>
{
    public Guid Id { get; } = Guid.NewGuid();
    public State State { get; protected set; } = State.NotStarted;
    public T Value { get; protected set; }
    public CancellationToken Token { get; protected set; }

    public bool IsComplete => IsSuccess || IsError || IsCancelled;
    public bool IsStarted => State == State.Started;
    public bool IsSuccess => State == State.Success;
    public bool IsError => State == State.Error;
    public bool IsCancelled => State == State.Cancelled;

    protected readonly TaskCompletionSource<T> _source;
    protected readonly List<(Guid, IConsumer<T>)> _consumers = [];

    public FutureBase(CancellationToken cancellation = default)
    {
        Token = cancellation;
        _source = new(cancellation);
        Value = default!;
    }

    public FutureBase(T value, CancellationToken cancellation = default)
    {
        Token = cancellation;
        _source = new(cancellation);
        Value = value;
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

    public Subscription Subscribe(IConsumer<T> consumer)
    {
        var id = Guid.NewGuid();
        var subscription = new Subscription(() => UnSubscribe(id));
        _consumers.Add((id, consumer));
        return subscription;
    }

    public IFuture<TNext> Pipe<TNext>(Func<T, TNext> next)
    {
        return new Future<T, TNext>(v => next(v), Token);
    }

    public IFuture<TNext> Pipe<TNext>(Func<T, Task<TNext>> next)
    {
        return new Future<T, TNext>(v => next(v).ConfigureAwait(false).GetAwaiter().GetResult());
    }

    public IFuture<TNext> Pipe<TNext>(Func<T, IFuture<TNext>> next)
    {
        return new Future<T, TNext>(v => next(v).Resolve());
    }

    public IFuture<T> Pipe(ITopic<T> topic)
    {
        return new Future<T>(v =>
        {
            topic.Next(v);
            return v;
        });
    }

    public IFuture<T> Pipe(ITopic<Task<T>> topic)
    {
        return new Future<T>(v =>
        {
            topic.Next(Task.FromResult(v));
            return v;
        });
    }

    public IFuture<TNext> Pipe<TNext>(ITransformer<T, TNext> next)
    {
        return new Future<T, TNext>(next.Next);
    }

    public IFuture<TNext> Pipe<TNext>(ITransformer<T, Task<TNext>> next)
    {
        return new Future<T, TNext>(next.Next);
    }

    internal void UnSubscribe(Guid id)
    {
        var i = _consumers.FindIndex(s => s.Item1 == id);

        if (i == -1)
        {
            return;
        }

        _consumers.RemoveAt(i);
    }
}