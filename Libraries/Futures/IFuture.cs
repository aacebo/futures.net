namespace Futures;

/// <summary>
/// an async value that can be resolved and subscribed to
/// </summary>
/// <typeparam name="T">the type being resolved/emitted</typeparam>
public interface IFuture<T> : IDisposable, IAsyncDisposable, IEnumerable<T>, IAsyncEnumerable<T>
{
    T Resolve();
    Task<T> AsTask();
    ValueTask<T> AsValueTask();
    Subscription Subscribe(IConsumer<T> consumer);

    IFuture<TNext> Pipe<TNext>(Func<T, TNext> next);
    IFuture<TNext> Pipe<TNext>(Func<T, Task<TNext>> next);
    IFuture<T> Pipe(IFuture<T> topic);
    IFuture<TNext> Pipe<TNext>(ITransformer<T, TNext> transform);
}