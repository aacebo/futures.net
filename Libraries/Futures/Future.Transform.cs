namespace Futures;

/// <summary>
/// a stream of data that returns
/// its latest output
/// </summary>
public partial class Future<T, TOut> : ICloneable, IStreamable<TOut>
{
    public Guid Id { get; } = Guid.NewGuid();
    public TOut? Value => Out.Value;

    protected Fn<T, TOut> Selector { get; set; }
    protected Future<T> In { get; set; }
    protected Future<TOut> Out { get; set; }

    public Future(Action<T, Future<TOut>> select, CancellationToken cancellation = default)
    {
        In ??= new(cancellation);
        Out ??= new(cancellation);
        Selector = new(value =>
        {
            var @out = new Future<TOut>();
            select(value, @out);
            return @out.Resolve();
        });
    }

    public Future(Func<T, Future<TOut>, Task> select, CancellationToken cancellation = default)
    {
        In ??= new(cancellation);
        Out ??= new(cancellation);
        Selector = new(value =>
        {
            var @out = new Future<TOut>();
            select(value, @out).ConfigureAwait(false);
            return @out.Resolve();
        });
    }

    public Future(Func<T, TOut> select, CancellationToken cancellation = default)
    {
        In ??= new(cancellation);
        Out ??= new(cancellation);
        Selector = select;
    }

    public Future(Func<T, Task<TOut>> select, CancellationToken cancellation = default)
    {
        In ??= new(cancellation);
        Out ??= new(cancellation);
        Selector = select;
    }

    public TOut Next(T value)
    {
        return Next(this, value);
    }

    internal TOut Next(object sender, T value)
    {
        var @out = Selector.Invoke(In.Next(sender, value));
        return Out.Next(sender, @out);
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
        return new Future<T, TOut>(Selector.Invoke)
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
        var prev = Selector;
        Selector = new(v => selector(v, prev));
        return this;
    }

    public Future<T, TOut> Wrap(Func<T, Func<T, TOut>, Task<TOut>> selector)
    {
        var prev = Selector;
        Selector = new(v => selector(v, prev));
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
}

/// <summary>
/// a stream of data that returns
/// its latest output
/// </summary>
public partial class Future<T1, T2, TOut> : ICloneable, IStreamable<TOut>
{
    public Guid Id { get; } = Guid.NewGuid();
    public TOut? Value => Out.Value;

    protected Fn<T1, T2, TOut> Selector { get; set; }
    protected Future<(T1, T2)> In { get; set; }
    protected Future<TOut> Out { get; set; }

    public Future(Action<T1, T2, Future<TOut>> select, CancellationToken cancellation = default)
    {
        In ??= new(cancellation);
        Out ??= new(cancellation);
        Selector = new((a, b) =>
        {
            var @out = new Future<TOut>();
            select(a, b, @out);
            return @out.Resolve();
        });
    }

    public Future(Func<T1, T2, Future<TOut>, Task> select, CancellationToken cancellation = default)
    {
        In ??= new(cancellation);
        Out ??= new(cancellation);
        Selector = new((a, b) =>
        {
            var @out = new Future<TOut>();
            select(a, b, @out).ConfigureAwait(false);
            return @out.Resolve();
        });
    }

    public Future(Func<T1, T2, TOut> select, CancellationToken cancellation = default)
    {
        In ??= new(cancellation);
        Out ??= new(cancellation);
        Selector = select;
    }

    public Future(Func<T1, T2, Task<TOut>> select, CancellationToken cancellation = default)
    {
        In ??= new(cancellation);
        Out ??= new(cancellation);
        Selector = select;
    }

    public TOut Next(T1 a, T2 b)
    {
        return Next(this, a, b);
    }

    internal TOut Next(object sender, T1 a, T2 b)
    {
        (a, b) = In.Next(sender, (a, b));
        var @out = Selector.Invoke(a, b);
        return Out.Next(sender, @out);
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
        return new Future<T1, T2, TOut>(Selector.Invoke)
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
        var prev = Selector;
        Selector = new((a, b) => selector(a, b, prev));
        return this;
    }

    public Future<T1, T2, TOut> Wrap(Func<T1, T2, Func<T1, T2, TOut>, Task<TOut>> selector)
    {
        var prev = Selector;
        Selector = new((a, b) => selector(a, b, prev));
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
}