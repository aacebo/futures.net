namespace Futures;

/// <summary>
/// a <c>Future</c> that can be written to
/// </summary>
/// <typeparam name="T">the type being written</typeparam>
public interface ITopic<T> : IFuture<T>, IConsumer<T>, IProducer<T>
{
    ITopic<TNext> Map<TNext>(Func<T, TNext> next);
    ITopic<TNext> Map<TNext>(Func<T, Task<TNext>> next);
    ITopic<T> Map(IFuture<T> topic);
    ITopic<TNext> Map<TNext>(ITransformer<T, TNext> transform);
}