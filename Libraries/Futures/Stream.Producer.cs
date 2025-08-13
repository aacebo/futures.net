namespace Futures;

public partial class Stream<T> : IProducer<T>
{
    public virtual ISubscription Subscribe(IConsumer<T> consumer)
    {
        var id = Guid.NewGuid();
        var teardown = _subscribe(consumer);
        var subscriber = new Subscriber<T>(consumer);
        subscriber.AddTearDown(() => UnSubscribe(id));
        subscriber.AddTearDown(teardown);
        _consumers.Add((id, subscriber));
        return subscriber;
    }

    public virtual ISubscription Subscribe(Action<T>? next = null, Action? complete = null, Action<Exception>? error = null, Action? cancel = null)
    {
        var id = Guid.NewGuid();
        var subscriber = new Subscriber<T>()
        {
            OnNext = next,
            OnComplete = complete,
            OnError = error,
            OnCancel = cancel
        };

        var teardown = _subscribe(subscriber);
        subscriber.AddTearDown(() => UnSubscribe(id));
        subscriber.AddTearDown(teardown);
        _consumers.Add((id, subscriber));
        return subscriber;
    }
}