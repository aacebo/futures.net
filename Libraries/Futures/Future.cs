namespace Futures;

/// <summary>
/// a stream of data that returns
/// its latest output
/// </summary>
public partial class Future<T> : Stream<T>
{
    public T? Value => Last;

    protected T? Last { get; set; }
    protected Fn<T, T> Transform { get; set; } = new(v => v);

    private readonly TaskCompletionSource<T> _task;

    public Future(CancellationToken cancellation = default) : base(cancellation)
    {
        _task = new(cancellation);
    }

    public Future(Stream<T> stream) : base(stream)
    {
        _task = new(stream.Token);
    }

    public Future(Action<T, Future<T>> transform, CancellationToken cancellation = default) : base(cancellation)
    {
        _task = new(cancellation);
        Transform = new(value =>
        {
            var @out = new Future<T>();
            transform(value, @out);
            return @out.Resolve();
        });
    }

    public Future(Func<T, Future<T>, Task> transform, CancellationToken cancellation = default) : base(cancellation)
    {
        _task = new(cancellation);
        Transform = new(value =>
        {
            var @out = new Future<T>();
            transform(value, @out);
            return @out.Resolve();
        });
    }

    public Future(Func<T, T> transform, CancellationToken cancellation = default) : base(cancellation)
    {
        _task = new(cancellation);
        Transform = transform;
    }

    public Future(Func<T, Task<T>> transform, CancellationToken cancellation = default) : base(cancellation)
    {
        _task = new(cancellation);
        Transform = transform;
    }

    public T Next(T value)
    {
        value = Transform.Invoke(value);
        Emit(value);
        Last = value;
        return value;
    }

    public T Complete()
    {
        if (Value is null)
        {
            throw new InvalidOperationException("attempt to call Future<T>.Complete() on empty future");
        }

        Success();
        _task.TrySetResult(Value);
        return Value;
    }

    public override void Error(Exception error)
    {
        base.Error(error);
        _task.TrySetException(error);
    }

    public override void Cancel()
    {
        base.Cancel();
        _task.TrySetCanceled();
    }

    public T Resolve()
    {
        return _task.Task.ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Task<T> AsTask()
    {
        return _task.Task;
    }

    public ValueTask<T> AsValueTask()
    {
        return new ValueTask<T>(_task.Task);
    }

    public Future<T> Pipe(IOperator<T> @operator)
    {
        return @operator.Invoke(this);
    }

    public Future<T, TNext> Pipe<TNext>(ITransformer<T, TNext> @operator)
    {
        return @operator.Invoke(this);
    }

    public override ISubscription Subscribe(Future<T> future)
    {
        return base.Subscribe(future);
    }

    public override ISubscription Subscribe<TNext>(Future<T, TNext> future)
    {
        return base.Subscribe(future);
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

    public static Future<T> From(Stream<T> stream)
    {
        return new(stream);
    }

    public static Future<T> From(IEnumerable<T> enumerable)
    {
        return Run(future =>
        {
            foreach (var item in enumerable)
            {
                future.Next(item);
            }

            future.Complete();
        });
    }

    public static Future<T> From(IAsyncEnumerable<T> enumerable)
    {
        return Run(async future =>
        {
            await foreach (var item in enumerable)
            {
                future.Next(item);
            }

            future.Complete();
        });
    }

    public static Future<T> Run(Action<Future<T>> onInit, CancellationToken cancellation = default)
    {
        var future = new Future<T>(cancellation);
        onInit(future);
        return future;
    }

    public static Future<T> Run(Func<Future<T>, Task> onInit, CancellationToken cancellation = default)
    {
        var future = new Future<T>(cancellation);
        onInit(future).ConfigureAwait(false);
        return future;
    }
}

/// <summary>
/// a stream of data that returns
/// its latest output
/// </summary>
public partial class Future<T, TOut> : Stream<TOut>
{
    public TOut? Value => Last;

    protected TOut? Last { get; set; }
    protected Fn<T, TOut> Transform { get; set; }

    private readonly TaskCompletionSource<TOut> _task;

    public Future(Action<T, Future<TOut>> transform, CancellationToken cancellation = default) : base(cancellation)
    {
        _task = new(cancellation);
        Transform = new(value =>
        {
            var @out = new Future<TOut>();
            transform(value, @out);
            return @out.Resolve();
        });
    }

    public Future(Func<T, Future<TOut>, Task> transform, CancellationToken cancellation = default) : base(cancellation)
    {
        _task = new(cancellation);
        Transform = new(value =>
        {
            var @out = new Future<TOut>();
            transform(value, @out).ConfigureAwait(false);
            return @out.Resolve();
        });
    }

    public Future(Func<T, TOut> transform, CancellationToken cancellation = default) : base(cancellation)
    {
        _task = new(cancellation);
        Transform = transform;
    }

    public Future(Func<T, Task<TOut>> transform, CancellationToken cancellation = default) : base(cancellation)
    {
        _task = new(cancellation);
        Transform = transform;
    }

    public TOut Next(T value)
    {
        var @out = Transform.Invoke(value);
        Emit(@out);
        Last = @out;
        return @out;
    }

    public TOut Complete()
    {
        if (Value is null)
        {
            throw new InvalidOperationException("attempt to call Future<T, TOut>.Complete() on empty future");
        }

        Success();
        _task.TrySetResult(Value);
        return Value;
    }

    public override void Error(Exception error)
    {
        base.Error(error);
        _task.TrySetException(error);
    }

    public override void Cancel()
    {
        base.Cancel();
        _task.TrySetCanceled();
    }

    public TOut Resolve()
    {
        return _task.Task.ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Task<TOut> AsTask()
    {
        return _task.Task;
    }

    public ValueTask<TOut> AsValueTask()
    {
        return new ValueTask<TOut>(_task.Task);
    }

    public Future<TOut> AsFuture()
    {
        var future = new Future<TOut>(Token);
        Subscribe(future);
        return future;
    }

    public Future<T, TOut> Pipe(IOperator<T, TOut> @operator)
    {
        return @operator.Invoke(this);
    }

    public Future<T, TNext> Pipe<TNext>(ITransformer<T, TOut, TNext> @operator)
    {
        return @operator.Invoke(this);
    }

    public override ISubscription Subscribe(Future<TOut> future)
    {
        return base.Subscribe(future);
    }

    public override ISubscription Subscribe<TNext>(Future<TOut, TNext> future)
    {
        return base.Subscribe(future);
    }
}

/// <summary>
/// a stream of data that returns
/// its latest output
/// </summary>
public partial class Future<T1, T2, TOut> : Stream<TOut>
{
    public TOut? Value => Last;

    protected TOut? Last { get; set; }
    protected Fn<T1, T2, TOut> Transform { get; set; }

    private readonly TaskCompletionSource<TOut> _task;

    public Future(Action<T1, T2, Future<TOut>> transform, CancellationToken cancellation = default) : base(cancellation)
    {
        _task = new(cancellation);
        Transform = new((a, b) =>
        {
            var @out = new Future<TOut>();
            transform(a, b, @out);
            return @out.Resolve();
        });
    }

    public Future(Func<T1, T2, Future<TOut>, Task> transform, CancellationToken cancellation = default) : base(cancellation)
    {
        _task = new(cancellation);
        Transform = new((a, b) =>
        {
            var @out = new Future<TOut>();
            transform(a, b, @out).ConfigureAwait(false);
            return @out.Resolve();
        });
    }

    public Future(Func<T1, T2, TOut> transform, CancellationToken cancellation = default) : base(cancellation)
    {
        _task = new(cancellation);
        Transform = transform;
    }

    public Future(Func<T1, T2, Task<TOut>> transform, CancellationToken cancellation = default) : base(cancellation)
    {
        _task = new(cancellation);
        Transform = transform;
    }

    public TOut Next(T1 a, T2 b)
    {
        var @out = Transform.Invoke(a, b);
        Emit(@out);
        Last = @out;
        return @out;
    }

    public TOut Complete()
    {
        if (Value is null)
        {
            throw new InvalidOperationException("attempt to call Future<T1, T2, TOut>.Complete() on empty future");
        }

        Success();
        _task.TrySetResult(Value);
        return Value;
    }

    public override void Error(Exception error)
    {
        base.Error(error);
        _task.TrySetException(error);
    }

    public override void Cancel()
    {
        base.Cancel();
        _task.TrySetCanceled();
    }

    public TOut Resolve()
    {
        return _task.Task.ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Task<TOut> AsTask()
    {
        return _task.Task;
    }

    public ValueTask<TOut> AsValueTask()
    {
        return new ValueTask<TOut>(_task.Task);
    }

    public Future<TOut> AsFuture()
    {
        var future = new Future<TOut>(Token);
        Subscribe(future);
        return future;
    }

    public Future<T1, T2, TOut> Pipe(IOperator<T1, T2, TOut> @operator)
    {
        return @operator.Invoke(this);
    }

    public Future<T1, T2, TNext> Pipe<TNext>(ITransformer<T1, T2, TOut, TNext> @operator)
    {
        return @operator.Invoke(this);
    }

    public override ISubscription Subscribe(Future<TOut> future)
    {
        return base.Subscribe(future);
    }

    public override ISubscription Subscribe<TNext>(Future<TOut, TNext> future)
    {
        return base.Subscribe(future);
    }
}