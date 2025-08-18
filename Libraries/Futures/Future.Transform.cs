namespace Futures;

/// <summary>
/// a stream of data that returns
/// its latest output
/// </summary>
public partial class Future<T, TOut> : ICloneable, IStreamable<TOut>
{
    public Guid Id { get; } = Guid.NewGuid();
    public TOut? Value => Out.Value;

    protected Fn<T, TOut> Transform { get; set; }
    protected Future<T> In { get; set; }
    protected Future<TOut> Out { get; set; }

    public Future(Action<T, Future<TOut>> transform, CancellationToken cancellation = default)
    {
        In ??= new(cancellation);
        Out ??= new(cancellation);
        Transform = new(value =>
        {
            var @out = new Future<TOut>();
            transform(value, @out);
            return @out.Resolve();
        });

        In.Subscribe(Subscriber<T>.From(this));
    }

    public Future(Func<T, Future<TOut>, Task> transform, CancellationToken cancellation = default)
    {
        In ??= new(cancellation);
        Out ??= new(cancellation);
        Transform = new(value =>
        {
            var @out = new Future<TOut>();
            transform(value, @out).ConfigureAwait(false);
            return @out.Resolve();
        });

        In.Subscribe(Subscriber<T>.From(this));
    }

    public Future(Func<T, TOut> transform, CancellationToken cancellation = default)
    {
        In ??= new(cancellation);
        Out ??= new(cancellation);
        Transform = transform;
        In.Subscribe(Subscriber<T>.From(this));
    }

    public Future(Func<T, Task<TOut>> transform, CancellationToken cancellation = default)
    {
        In ??= new(cancellation);
        Out ??= new(cancellation);
        Transform = transform;
        In.Subscribe(Subscriber<T>.From(this));
    }

    public TOut Next(T value)
    {
        return Next(this, value);
    }

    internal TOut Next(object _, T value)
    {
        var @out = Transform.Invoke(In.Next(this, value));
        return Out.Next(this, @out);
    }

    public TOut Complete()
    {
        return Out.Complete();
    }

    public void Error(Exception error)
    {
        Out.Error(error);
    }

    public void Cancel()
    {
        Out.Cancel();
    }

    public TOut Resolve()
    {
        return Out.Resolve();
    }

    public Task<TOut> AsTask()
    {
        return Out.AsTask();
    }

    public ValueTask<TOut> AsValueTask()
    {
        return Out.AsValueTask();
    }

    public Future<TOut> Fork()
    {
        return Out.Fork();
    }

    public Future<T, TOut> Clone()
    {
        return new Future<T, TOut>(Transform.Invoke)
        {
            In = In,
            Out = Out,
        };
    }

    object ICloneable.Clone()
    {
        return Clone();
    }

    public Future<T, TOut> Wrap(Func<T, Func<T, TOut>, TOut> selector)
    {
        var prev = Transform;
        Transform = new(v => selector(v, prev));
        return this;
    }

    public Future<T, TOut> Wrap(Func<T, Func<T, TOut>, Task<TOut>> selector)
    {
        var prev = Transform;
        Transform = new(v => selector(v, prev));
        return this;
    }

    public Future<T, TOut> Pipe(IOperator<T, TOut> @operator)
    {
        return @operator.Invoke(this);
    }

    public Future<T, TNext> Pipe<TNext>(ITransformer<T, TOut, TNext> @operator)
    {
        return @operator.Invoke(this);
    }

    public static Future<T, TOut> Run(Action<Future<T, TOut>> onInit, Func<T, TOut> transform, CancellationToken cancellation = default)
    {
        var future = new Future<T, TOut>(transform, cancellation);
        onInit(future);
        return future;
    }

    public static Future<T, TOut> Run(Func<Future<T, TOut>, Task> onInit, Func<T, TOut> transform, CancellationToken cancellation = default)
    {
        var future = new Future<T, TOut>(transform, cancellation);
        onInit(future).ConfigureAwait(false);
        return future;
    }

    public static Future<T, TOut> Run(Action<Future<T, TOut>> onInit, Func<T, Task<TOut>> transform, CancellationToken cancellation = default)
    {
        var future = new Future<T, TOut>(transform, cancellation);
        onInit(future);
        return future;
    }

    public static Future<T, TOut> Run(Func<Future<T, TOut>, Task> onInit, Func<T, Task<TOut>> transform, CancellationToken cancellation = default)
    {
        var future = new Future<T, TOut>(transform, cancellation);
        onInit(future).ConfigureAwait(false);
        return future;
    }
}

/// <summary>
/// a stream of data that returns
/// its latest output
/// </summary>
public partial class Future<T1, T2, TOut> : ICloneable, IStreamable<TOut>
{
    public Guid Id { get; } = Guid.NewGuid();
    public TOut? Value => Out.Value;

    protected Fn<T1, T2, TOut> Transform { get; set; }
    protected Future<(T1, T2)> In { get; set; }
    protected Future<TOut> Out { get; set; }

    public Future(Action<T1, T2, Future<TOut>> transform, CancellationToken cancellation = default)
    {
        In ??= new(cancellation);
        Out ??= new(cancellation);
        Transform = new((a, b) =>
        {
            var @out = new Future<TOut>();
            transform(a, b, @out);
            return @out.Resolve();
        });
    }

    public Future(Func<T1, T2, Future<TOut>, Task> transform, CancellationToken cancellation = default)
    {
        In ??= new(cancellation);
        Out ??= new(cancellation);
        Transform = new((a, b) =>
        {
            var @out = new Future<TOut>();
            transform(a, b, @out).ConfigureAwait(false);
            return @out.Resolve();
        });
    }

    public Future(Func<T1, T2, TOut> transform, CancellationToken cancellation = default)
    {
        In ??= new(cancellation);
        Out ??= new(cancellation);
        Transform = transform;
    }

    public Future(Func<T1, T2, Task<TOut>> transform, CancellationToken cancellation = default)
    {
        In ??= new(cancellation);
        Out ??= new(cancellation);
        Transform = transform;
    }

    public TOut Next(T1 a, T2 b)
    {
        return Next(this, a, b);
    }

    internal TOut Next(object _, T1 a, T2 b)
    {
        (a, b) = In.Next(this, (a, b));
        var @out = Transform.Invoke(a, b);
        return Out.Next(this, @out);
    }

    public TOut Complete()
    {
        return Out.Complete();
    }

    public void Error(Exception error)
    {
        Out.Error(error);
    }

    public void Cancel()
    {
        Out.Cancel();
    }

    public TOut Resolve()
    {
        return Out.Resolve();
    }

    public Task<TOut> AsTask()
    {
        return Out.AsTask();
    }

    public ValueTask<TOut> AsValueTask()
    {
        return Out.AsValueTask();
    }

    public Future<TOut> Fork()
    {
        return Out.Fork();
    }

    public Future<T1, T2, TOut> Clone()
    {
        return new Future<T1, T2, TOut>(Transform.Invoke)
        {
            In = In,
            Out = Out,
        };
    }

    object ICloneable.Clone()
    {
        return Clone();
    }

    public Future<T1, T2, TOut> Wrap(Func<T1, T2, Func<T1, T2, TOut>, TOut> selector)
    {
        var prev = Transform;
        Transform = new((a, b) => selector(a, b, prev));
        return this;
    }

    public Future<T1, T2, TOut> Wrap(Func<T1, T2, Func<T1, T2, TOut>, Task<TOut>> selector)
    {
        var prev = Transform;
        Transform = new((a, b) => selector(a, b, prev));
        return this;
    }

    public Future<T1, T2, TOut> Pipe(IOperator<T1, T2, TOut> @operator)
    {
        return @operator.Invoke(this);
    }

    public Future<T1, T2, TNext> Pipe<TNext>(ITransformer<T1, T2, TOut, TNext> @operator)
    {
        return @operator.Invoke(this);
    }

    public static Future<T1, T2, TOut> Run(Action<Future<T1, T2, TOut>> onInit, Func<T1, T2, TOut> transform, CancellationToken cancellation = default)
    {
        var future = new Future<T1, T2, TOut>(transform, cancellation);
        onInit(future);
        return future;
    }

    public static Future<T1, T2, TOut> Run(Func<Future<T1, T2, TOut>, Task> onInit, Func<T1, T2, TOut> transform, CancellationToken cancellation = default)
    {
        var future = new Future<T1, T2, TOut>(transform, cancellation);
        onInit(future).ConfigureAwait(false);
        return future;
    }

    public static Future<T1, T2, TOut> Run(Action<Future<T1, T2, TOut>> onInit, Func<T1, T2, Task<TOut>> transform, CancellationToken cancellation = default)
    {
        var future = new Future<T1, T2, TOut>(transform, cancellation);
        onInit(future);
        return future;
    }

    public static Future<T1, T2, TOut> Run(Func<Future<T1, T2, TOut>, Task> onInit, Func<T1, T2, Task<TOut>> transform, CancellationToken cancellation = default)
    {
        var future = new Future<T1, T2, TOut>(transform, cancellation);
        onInit(future).ConfigureAwait(false);
        return future;
    }
}