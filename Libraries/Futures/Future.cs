using System.Collections;

namespace Futures;

public partial class Future<T> : IFuture<T>
{
    public Guid Id { get; } = Guid.NewGuid();
    public State State { get; protected set; } = State.NotStarted;
    public CancellationToken Token { get; protected set; }
    public T Value { get; protected set; } = default!;

    public bool IsComplete => IsSuccess || IsError || IsCancelled;
    public bool IsStarted => State == State.Started;
    public bool IsSuccess => State == State.Success;
    public bool IsError => State == State.Error;
    public bool IsCancelled => State == State.Cancelled;

    protected readonly TaskCompletionSource<T> _source;
    protected readonly List<(Guid, ISubscription, IConsumer<T>)> _consumers = [];
    protected readonly Subscriber _subscribe;

    public Future(CancellationToken cancellation = default)
    {
        Token = cancellation;
        _source = new(cancellation);
        _subscribe = (_, _) => () => { };
    }

    public Future(Action<IFuture<T>, IConsumer<T>> subscribe, CancellationToken cancellation = default)
    {
        Token = cancellation;
        _source = new(cancellation);
        _subscribe = (source, consumer) =>
        {
            subscribe(source, consumer);
            return () => { };
        };
    }

    public Future(Func<IFuture<T>, IConsumer<T>, Action> subscribe, CancellationToken cancellation = default)
    {
        Token = cancellation;
        _source = new(cancellation);
        _subscribe = (source, consumer) => subscribe(source, consumer);
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

    public ISubscription Subscribe(IConsumer<T> consumer)
    {
        var id = Guid.NewGuid();
        var teardown = _subscribe(this, consumer);
        var subscription = new Subscription<T>(() =>
        {
            UnSubscribe(id);
            teardown();
        });

        _consumers.Add((id, subscription, consumer));
        return subscription;
    }

    public ISubscription Subscribe(
        Action<T>? next = null,
        Action? complete = null,
        Action<Exception>? error = null,
        Action? cancel = null
    )
    {
        var id = Guid.NewGuid();
        var consumer = new Consumer<T>()
        {
            OnNext = next,
            OnComplete = complete,
            OnError = error,
            OnCancel = cancel
        };

        var teardown = _subscribe(this, consumer);
        var subscription = new Subscription<T>(() =>
        {
            UnSubscribe(id);
            teardown();
        });

        _consumers.Add((id, subscription, consumer));
        return subscription;
    }

    private void UnSubscribe(Guid id)
    {
        var i = _consumers.FindIndex(s => s.Item1 == id);

        if (i == -1)
        {
            return;
        }

        _consumers.RemoveAt(i);
    }

    public IEnumerator<T> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public delegate Action Subscriber(IFuture<T> destination, IConsumer<T> consumer);

    public static Future<T> From(T value)
    {
        return new Future<T>((_, consumer) =>
        {
            consumer.Next(value);
            consumer.Complete();
        });
    }

    public static Future<T> From(Task<T> task)
    {
        return new Future<T>(async (_, consumer) =>
        {
            try
            {
                consumer.Next(await task);
                consumer.Complete();
            }
            catch (Exception err)
            {
                consumer.Error(err);
            }
        });
    }
}