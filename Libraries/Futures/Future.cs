namespace Futures;

public partial class Future<T> : ISubscribable<T>
{
    public Guid Id { get; } = Guid.NewGuid();
    public State State { get; protected set; } = State.NotStarted;
    public T Value { get; protected set; } = default!;
    public CancellationToken Token { get; protected set; }

    public bool IsComplete => IsSuccess || IsError || IsCancelled;
    public bool IsStarted => State == State.Started;
    public bool IsSuccess => State == State.Success;
    public bool IsError => State == State.Error;
    public bool IsCancelled => State == State.Cancelled;

    protected readonly TaskCompletionSource<T> _source;
    protected readonly List<(Guid, Consumer<T>)> _consumers = [];

    private readonly Func<T, T> _resolver;

    public Future(CancellationToken cancellation = default)
    {
        Token = cancellation;
        _source = new(cancellation);
        _resolver = value => value;
    }

    public Future(Func<T, T> resolver, CancellationToken cancellation = default)
    {
        Token = cancellation;
        _source = new(cancellation);
        _resolver = resolver;
    }

    public Future(Func<T, Task<T>> resolver, CancellationToken cancellation = default)
    {
        Token = cancellation;
        _source = new(cancellation);
        _resolver = value => resolver(value).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Future(Func<T, Future<T>> resolver, CancellationToken cancellation = default)
    {
        Token = cancellation;
        _source = new(cancellation);
        _resolver = value => resolver(value).Resolve();
    }

    public Future(Func<T, Task<Future<T>>> resolver, CancellationToken cancellation = default)
    {
        Token = cancellation;
        _source = new(cancellation);
        _resolver = value => resolver(value).ConfigureAwait(false).GetAwaiter().GetResult().Resolve();
    }

    public Future(Action<T, Producer<T>> resolver, CancellationToken cancellation = default)
    {
        Token = cancellation;
        _source = new(cancellation);
        _resolver = value =>
        {
            var future = new Future<T>(Token);
            resolver(value, new(future));
            return future.Resolve();
        };
    }

    public Future(Func<T, Producer<T>, Task> resolver, CancellationToken cancellation = default)
    {
        Token = cancellation;
        _source = new(cancellation);
        _resolver = value =>
        {
            var future = new Future<T>(Token);
            resolver(value, new(future)).ConfigureAwait(false).GetAwaiter().GetResult();
            return future.Resolve();
        };
    }

    ~Future()
    {
        Dispose();
    }

    internal T Next(T value)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Started;
        value = _resolver(value);
        Value = value;

        foreach (var (_, consumer) in _consumers)
        {
            consumer.Next(value);
        }

        return value;
    }

    internal Task<T> NextAsync(T value)
    {
        return Task.FromResult(Next(value));
    }

    internal T Complete()
    {
        if (Value is null)
        {
            throw new InvalidOperationException("attempted to complete a future with no data");
        }

        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Success;
        _source.TrySetResult(Value);

        foreach (var (_, consumer) in _consumers)
        {
            consumer.Complete();
        }

        return Value;
    }

    internal Task<T> CompleteAsync()
    {
        return Task.FromResult(Complete());
    }

    internal void Error(Exception ex)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Error;
        _source.TrySetException(ex);

        foreach (var (_, consumer) in _consumers)
        {
            consumer.Error(ex);
        }
    }

    internal Task ErrorAsync(Exception ex)
    {
        Error(ex);
        return Task.CompletedTask;
    }

    internal void Cancel()
    {
        if (IsComplete) return;

        State = State.Cancelled;
        _source.TrySetCanceled();

        foreach (var (_, consumer) in _consumers)
        {
            consumer.Cancel();
        }
    }

    internal Task CancelAsync()
    {
        Cancel();
        return Task.CompletedTask;
    }

    public T Resolve()
    {
        return _source.Task.ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Task<T> ResolveAsync()
    {
        return _source.Task;
    }

    public ValueTask<T> AsValueTask()
    {
        return new ValueTask<T>(_source.Task);
    }

    public Subscription Subscribe(Consumer<T> consumer)
    {
        var id = Guid.NewGuid();
        var subscription = new Subscription(() => UnSubscribe(id));
        consumer.Subscription = subscription;
        _consumers.Add((id, consumer));
        return subscription;
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

    public Future<T, T> ToProducer()
    {
        return new Future<T, T>(Next);
    }

    public Future<T, TNext> Pipe<TNext>(Func<T, TNext> next)
    {
        return new Future<T, TNext>(value => next(Next(value)), Token);
    }

    public Future<T, TNext> Pipe<TNext>(Func<T, Task<TNext>> next)
    {
        return new Future<T, TNext>(value =>
        {
            return next(Next(value)).ConfigureAwait(false).GetAwaiter().GetResult();
        }, Token);
    }

    public Future<T, TNext> Pipe<TNext>(Func<T, Future<TNext>> next)
    {
        return new Future<T, TNext>(value => next(Next(value)).Resolve(), Token);
    }

    public Future<T, TNextOut> Pipe<TNext, TNextOut>(Func<T, Future<TNext, TNextOut>> next)
    {
        return new Future<T, TNextOut>(value => next(Next(value)).Resolve(), Token);
    }

    public Future<T, TNext> Pipe<TNext>(Func<T, Task<Future<TNext>>> next)
    {
        return new Future<T, TNext>(value =>
        {
            return next(Next(value)).ConfigureAwait(false).GetAwaiter().GetResult().Resolve();
        }, Token);
    }

    public Future<T, TNextOut> Pipe<TNext, TNextOut>(Func<T, Task<Future<TNext, TNextOut>>> next)
    {
        return new Future<T, TNextOut>(value =>
        {
            return next(Next(value)).ConfigureAwait(false).GetAwaiter().GetResult().Resolve();
        }, Token);
    }

    public static Future<T> From(T value)
    {
        var future = new Future<T>();
        future.Next(value);
        future.Complete();
        return future;
    }

    public static Future<T> From(Exception error)
    {
        var future = new Future<T>();
        future.Error(error);
        return future;
    }

    public static Future<T> From(IEnumerable<T> enmerable, CancellationToken cancellation = default)
    {
        var future = new Future<T>(cancellation);

        _ = Task.Run(() =>
        {
            foreach (var item in enmerable)
            {
                future.Next(item);
            }

            future.Complete();
        }, future.Token);

        return future;
    }

    public static Future<T> From(IAsyncEnumerable<T> enmerable, CancellationToken cancellation = default)
    {
        var future = new Future<T>(cancellation);

        _ = Task.Run(async () =>
        {
            await foreach (var item in enmerable)
            {
                future.Next(item);
            }

            future.Complete();
        }, future.Token);

        return future;
    }
}