namespace Futures;

public partial class Future<T> : IDisposable, IAsyncDisposable
{
    public void Dispose()
    {
        foreach (var (_, subscription, _) in _consumers)
        {
            subscription.UnSubscribe();
        }

        if (!IsComplete)
        {
            Cancel();
        }

        _source.Task.Dispose();
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return default;
    }
}