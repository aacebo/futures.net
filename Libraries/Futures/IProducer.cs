namespace Futures;

/// <summary>
/// produces some value that can be
/// subscribed to
/// </summary>
/// <typeparam name="TOut">the data that is produced</typeparam>
public interface IProducer<TOut>
{
    ISubscription Subscribe(IConsumer<TOut> consumer);
    ISubscription Subscribe(Action<TOut>? next = null, Action? complete = null, Action<Exception>? error = null, Action? cancel = null);
}