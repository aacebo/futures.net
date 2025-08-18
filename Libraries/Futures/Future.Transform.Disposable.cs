namespace Futures;

public partial class Future<T, TOut> : IDisposable, IAsyncDisposable
{
    public void Dispose()
    {
        In.Dispose();
        Out.Dispose();
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return default;
    }
}

public partial class Future<T1, T2, TOut> : IDisposable, IAsyncDisposable
{
    public void Dispose()
    {
        In.Dispose();
        Out.Dispose();
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return default;
    }
}