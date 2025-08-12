namespace Futures;

public partial class Future<T> : FutureBase<T>, IFuture<T>
{
    private readonly Func<T, T> _resolve;

    public Future(CancellationToken cancellation = default) : base(cancellation)
    {
        _resolve = value => value;
    }

    public Future(T value, CancellationToken cancellation = default) : base(cancellation)
    {
        Value = value;
        _resolve = value => value;
    }

    public Future(Func<T, T> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolve = resolve;
    }

    public Future(Func<T, Task<T>> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolve = value => resolve(value).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Future(ITopic<T>.Resolver resolver, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolve = value =>
        {
            var future = new Future<T>(cancellation);
            resolver(value, new Consumer<T>(future));
            return future.Resolve();
        };
    }

    ~Future()
    {
        Dispose();
    }

    public static Future<T> From(T value)
    {
        var future = new Future<T>();
        future.Next(value);
        future.Complete();
        return future;
    }

    public static Future<T> From(Exception error)
    {
        var future = new Future<T>();
        future.Error(error);
        return future;
    }
}

public partial class Future<T, TOut> : FutureBase<TOut>, IFuture<TOut>
{
    private readonly Func<T, TOut> _resolve;

    public Future(Func<T, TOut> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolve = resolve;
    }

    public Future(Func<T, Task<TOut>> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolve = value => resolve(value).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Future(ITransformer<T, TOut>.Resolver resolver, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolve = value =>
        {
            var future = new Future<TOut>(cancellation);
            resolver(value, new Consumer<TOut>(future));
            return future.Resolve();
        };
    }

    ~Future()
    {
        Dispose();
    }
}

public partial class Future<T1, T2, TOut> : FutureBase<TOut>, IFuture<TOut>
{
    private readonly Func<T1, T2, TOut> _resolve;

    public Future(Func<T1, T2, TOut> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolve = resolve;
    }

    public Future(Func<T1, T2, Task<TOut>> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolve = (a, b) => resolve(a, b).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Future(ITransformer<T1, T2, TOut>.Resolver resolver, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolve = (a, b) =>
        {
            var future = new Future<TOut>(cancellation);
            resolver(a, b, new Consumer<TOut>(future));
            return future.Resolve();
        };
    }

    ~Future()
    {
        Dispose();
    }
}