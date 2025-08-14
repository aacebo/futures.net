namespace Futures;

/// <summary>
/// produces some value that can be
/// subscribed to
/// </summary>
/// <typeparam name="TOut">the data that is produced</typeparam>
public interface IProducer<TOut> : IProducer<TOut, TOut>;

/// <summary>
/// produces some value that can be
/// subscribed to
/// </summary>
/// <typeparam name="TOut">the data that is produced</typeparam>
public interface IProducer<T, TOut>
{
    ISubscription Subscribe(IConsumer<T, TOut> consumer);
    ISubscription Subscribe(IConsumer<TOut, TOut> consumer);
    ISubscription Subscribe(Action<TOut>? next = null, Action? complete = null, Action<Exception>? error = null, Action? cancel = null);
    ISubscription Subscribe(Func<T, IEnumerable<TOut>>? next = null, Action? complete = null, Action<Exception>? error = null, Action? cancel = null);
}