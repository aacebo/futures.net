namespace Futures;

/// <summary>
/// a stream of data that returns
/// its latest output
/// </summary>
public partial class Future<T> : Stream<T>, IStreamable<T>, ICloneable, IEquatable<IIdentifiable>
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
        var @out = Transform.Invoke(value);
        Emit(sender, @out);
        Value = @out;
        return @out;
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
}