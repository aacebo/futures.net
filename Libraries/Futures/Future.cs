namespace Futures;

public partial class Future<T> : Stream<T>, IFuture<T>
{
    public T Value => Last ?? throw new NullReferenceException();

    public Future(CancellationToken cancellation = default) : base(cancellation)
    {

    }

    public Future(Action<IConsumer<T>> subscribe, CancellationToken cancellation = default) : base(subscribe, cancellation)
    {

    }

    public Future(Func<IConsumer<T>, Action> subscribe, CancellationToken cancellation = default) : base(subscribe, cancellation)
    {

    }

    public IFuture<T> Pipe(IOperator<T> @operator)
    {
        return @operator.Invoke(this);
    }

    public IFuture<TNext> Pipe<TNext>(IOperator<T, TNext> @operator)
    {
        return @operator.Invoke(this);
    }

    public static Future<T> From(T value)
    {
        return new Future<T>(destination =>
        {
            destination.Next(value);
            destination.Complete();
        });
    }

    public static Future<T> From(Task<T> task)
    {
        return new Future<T>(async destination =>
        {
            try
            {
                destination.Next(await task);
                destination.Complete();
            }
            catch (Exception err)
            {
                destination.Error(err);
            }
        });
    }

    public static Future<T> From(IEnumerable<T> enumerable)
    {
        return new Future<T>(destination =>
        {
            foreach (var item in enumerable)
            {
                destination.Next(item);
            }

            destination.Complete();
        });
    }

    public static Future<T> From(IAsyncEnumerable<T> enumerable)
    {
        return new Future<T>(async destination =>
        {
            await foreach (var item in enumerable)
            {
                destination.Next(item);
            }

            destination.Complete();
        });
    }
}

public partial class Future<T, TOut> : Stream<TOut>, IFuture<TOut>
{
    public TOut Value => Last ?? throw new NullReferenceException();

    private readonly Func<T, TOut> _selector;

    public Future(Func<T, TOut> select, CancellationToken cancellation = default) : base(cancellation)
    {
        _selector = select;
    }

    public Future(Func<T, Task<TOut>> select, CancellationToken cancellation = default) : base(cancellation)
    {
        _selector = value => select(value).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Future(Func<T, IFuture<TOut>> select, CancellationToken cancellation = default) : base(cancellation)
    {
        _selector = value => select(value).Resolve();
    }

    public IFuture<TOut> Pipe(IOperator<TOut> @operator)
    {
        return @operator.Invoke(this);
    }

    public IFuture<TNext> Pipe<TNext>(IOperator<TOut, TNext> @operator)
    {
        return @operator.Invoke(this);
    }
}

public partial class Future<T1, T2, TOut> : Stream<TOut>, IFuture<TOut>
{
    public TOut Value => Last ?? throw new NullReferenceException();

    private readonly Func<T1, T2, TOut> _selector;

    public Future(Func<T1, T2, TOut> select, CancellationToken cancellation = default) : base(cancellation)
    {
        _selector = select;
    }

    public Future(Func<T1, T2, Task<TOut>> select, CancellationToken cancellation = default) : base(cancellation)
    {
        _selector = (a, b) => select(a, b).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Future(Func<T1, T2, IFuture<TOut>> select, CancellationToken cancellation = default) : base(cancellation)
    {
        _selector = (a, b) => select(a, b).Resolve();
    }

    public IFuture<TOut> Pipe(IOperator<TOut> @operator)
    {
        return @operator.Invoke(this);
    }

    public IFuture<TNext> Pipe<TNext>(IOperator<TOut, TNext> @operator)
    {
        return @operator.Invoke(this);
    }
}