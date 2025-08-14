namespace Futures;

public partial class Future<T, TOut> : IProducer<T, TOut>
{
    public ISubscription Subscribe(Action<TOut>? next = null, Action? complete = null, Action<Exception>? error = null, Action? cancel = null)
    {
        var id = Guid.NewGuid();
        var subscriber = new Subscriber<TOut, TOut>()
        {
            Next = v =>
            {
                if (next is not null) next(v);
                return [];
            },
            Complete = complete,
            Error = error,
            Cancel = cancel
        };

        subscriber.AddTearDown(() => UnSubscribe(id));
        _listeners.Add((id, subscriber));
        return subscriber;
    }

    public ISubscription Subscribe(Func<T, IEnumerable<TOut>>? next = null, Action? complete = null, Action<Exception>? error = null, Action? cancel = null)
    {
        var id = Guid.NewGuid();
        var subscriber = new Subscriber<T, TOut>()
        {
            Next = v => next is not null ? next(v) : [],
            Complete = complete,
            Error = error,
            Cancel = cancel
        };

        subscriber.AddTearDown(() => UnSubscribe(id));
        _transformers.Add((id, subscriber));
        return subscriber;
    }

    public virtual ISubscription Subscribe(IConsumer<T, TOut> consumer)
    {
        var id = Guid.NewGuid();
        var subscriber = new Subscriber<T, TOut>(consumer);
        subscriber.AddTearDown(() => UnSubscribe(id));
        _transformers.Add((id, subscriber));
        return subscriber;
    }

    public ISubscription Subscribe(IConsumer<TOut, TOut> consumer)
    {
        var id = Guid.NewGuid();
        var subscriber = new Subscriber<TOut, TOut>(consumer);
        subscriber.AddTearDown(() => UnSubscribe(id));
        _listeners.Add((id, subscriber));
        return subscriber;
    }
}