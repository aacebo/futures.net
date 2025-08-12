namespace Futures;

public partial class Future<T> : IDisposable, IAsyncDisposable
{
    public void Dispose()
    {
        foreach (var (id, _, _) in _consumers)
        {
            UnSubscribe(id);
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