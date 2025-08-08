namespace Futures;

public partial class Future<T> : IDisposable, IAsyncDisposable
{
    public void Dispose()
    {
        foreach (var (_, consumer) in _consumers)
        {
            consumer.Dispose();
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