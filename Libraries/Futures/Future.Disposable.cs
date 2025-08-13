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

public partial class Future<T, TOut> : IDisposable, IAsyncDisposable
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

public partial class Future<T1, T2, TOut> : IDisposable, IAsyncDisposable
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