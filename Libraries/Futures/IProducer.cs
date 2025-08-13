namespace Futures;

/// <summary>
/// produces some value that can be
/// subscribed to
/// </summary>
/// <typeparam name="T">the data that is produced</typeparam>
public interface IProducer<out T>
{
    ISubscription Subscribe(IConsumer<T> consumer);
    ISubscription Subscribe(Action<T>? next = null, Action? complete = null, Action<Exception>? error = null, Action? cancel = null);
}
