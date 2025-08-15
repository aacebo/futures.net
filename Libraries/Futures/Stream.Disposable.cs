namespace Futures;

public partial class Stream<T> : IDisposable, IAsyncDisposable
{
    public virtual void Dispose()
    {
        foreach (var subscriber in Consumers)
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
