namespace Futures;

/// <summary>
/// a <c>Future</c> that can be written to
/// </summary>
/// <typeparam name="T">the type being written</typeparam>
public interface ITopic<T> : IFuture<T>, IConsumer<T>
{
    IStream<T, TNext> Map<TNext>(Func<T, TNext> next);
    IStream<T, TNext> Map<TNext>(Func<T, Task<TNext>> next);
    IStream<T, TNext> Map<TNext>(IStream<T, TNext> stream);

    public delegate void Resolver(T value, IConsumer<T> consumer);
}