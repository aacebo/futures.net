namespace Futures;

public partial class Future<T> : IDisposable, IAsyncDisposable
{
    public override void Dispose()
    {
        _source.Task.Dispose();
        base.Dispose();
    }

    public override ValueTask DisposeAsync()
    {
        Dispose();
        return default;
    }
}