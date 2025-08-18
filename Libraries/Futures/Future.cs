namespace Futures;

/// <summary>
/// a stream of data that returns
/// its latest output
/// </summary>
public partial class Future<T> : Stream<T>, IStreamable<T>, ICloneable
{
    public T? Value { get; protected set; }

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
        return Next(this, value);
    }

    internal T Next(object sender, T value)
    {
        value = Transform.Invoke(value);
        Emit(sender, value);
        Value = value;
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

    public Future<T> Fork()
    {
        var future = new Future<T>(Token);
        Subscribe(future);
        return future;
    }

    public Future<T> Clone()
    {
        var future = new Future<T>(Transform.Invoke);
        Wrap((value, select) => future.Next(value));
        return future;
    }

    object ICloneable.Clone()
    {
        return Clone();
    }

    public Future<T> Wrap(Func<T, Func<T, T>, T> selector)
    {
        var prev = Transform;
        Transform = new(v => selector(v, prev));
        return this;
    }

    public Future<T> Wrap(Func<T, Func<T, T>, Task<T>> selector)
    {
        var prev = Transform;
        Transform = new(v => selector(v, prev));
        return this;
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