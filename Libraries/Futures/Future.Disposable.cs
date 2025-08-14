namespace Futures;

public partial class Future<T, TOut> : IDisposable, IAsyncDisposable
{
    public virtual void Dispose()
    {
        foreach (var (_, subscriber) in _transformers)
        {
            subscriber.UnSubscribe();
        }

        foreach (var (_, subscriber) in _listeners)
        {
            subscriber.UnSubscribe();
        }

        if (!IsComplete)
        {
            Cancel();
        }

        GC.SuppressFinalize(this);
    }

    public virtual ValueTask DisposeAsync()
    {
        Dispose();
        return default;
    }
}
