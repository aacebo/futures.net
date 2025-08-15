namespace Futures;

public partial class Stream<T> : IProducer<T>
{
    public ISubscription Subscribe(IConsumer<T> consumer)
    {
        var subscriber = new Subscriber<T>(consumer);

        subscriber.AddTearDown(() =>
        {
            var i = Consumers.FindIndex(s => s.Id == subscriber.Id);

            if (i == -1) return;

            Consumers.RemoveAt(i);
        });

        Consumers.Add(subscriber);
        return subscriber;
    }

    public ISubscription Subscribe(Action<T>? next = null, Action? complete = null, Action<Exception>? error = null, Action? cancel = null)
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
}