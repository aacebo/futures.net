namespace Futures;

/// <summary>
/// a <c>Future</c> that can be written to
/// </summary>
/// <typeparam name="T">the type being written</typeparam>
public interface ITopic<T> : IFuture<T>, IConsumer<T>
{
    ITransformer<T, TNext> Map<TNext>(Func<T, TNext> next);
    ITransformer<T, TNext> Map<TNext>(Func<T, Task<TNext>> next);
    ITransformer<T, TNext> Map<TNext>(ITransformer<T, TNext> next);

    public delegate void Resolver(T value, IConsumer<T> consumer);
}