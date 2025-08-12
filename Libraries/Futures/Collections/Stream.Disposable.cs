namespace Futures.Collections;

public partial class Stream<T, TOut> : IDisposable, IAsyncDisposable
{
    public void Dispose()
    {
        foreach (var (id, _) in _consumers)
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