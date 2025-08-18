namespace Futures;

public partial class Stream<T> : IProducer<T>
{
    public ISubscription Subscribe(IConsumer<T> consumer)
    {
        var subscriber = consumer is Subscriber<T> s ? s : new Subscriber<T>(consumer);

        subscriber.AddTearDown(() =>
        {
            var i = Consumers.FindIndex(s => s.Id == subscriber.Id);

            if (i == -1) return;

            Consumers.RemoveAt(i);
        });

        Consumers.Add(subscriber);
        return subscriber;
    }

    public virtual ISubscription Subscribe(Future<T> future)
    {
        return Subscribe(new Subscriber<T>(future));
    }

    public virtual ISubscription Subscribe<TNext>(Future<T, TNext> future)
    {
        return Subscribe(Subscriber<T>.From(future));
    }

    public ISubscription Subscribe(Action<object, T>? next = null, Action? complete = null, Action<Exception>? error = null, Action? cancel = null)
    {
        var subscriber = new Subscriber<T>()
        {
            Next = next,
            Complete = complete,
            Error = error,
            Cancel = cancel
        };

        return Subscribe(subscriber);
    }

    public ISubscription Subscribe(Func<object, T, Task>? next = null, Action? complete = null, Action<Exception>? error = null, Action? cancel = null)
    {
        var subscriber = new Subscriber<T>()
        {
            Next = next is null ? null : (sender, value) => next(sender, value).ConfigureAwait(false),
            Complete = complete,
            Error = error,
            Cancel = cancel
        };

        return Subscribe(subscriber);
    }
}