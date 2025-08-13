namespace Futures.Operators;

public sealed class Do<T> : IOperator<T>
{
    private readonly Action<T> _selector;

    public Do(Action<T> select)
    {
        _selector = select;
    }

    public Do(Func<T, Task> select)
    {
        _selector = value => select(value).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public IFuture<T> Invoke(IFuture<T> source)
    {
        return new Future<T>(destination =>
        {
            source.Subscribe(new Subscriber<T>(destination)
            {
                OnNext = value =>
                {
                    _selector(value);
                    destination.Next(value);
                }
            });
        });
    }
}

public static partial class FutureExtensions
{
    public static IFuture<T> Do<T>(this IFuture<T> future, Action<T> select)
    {
        return future.Pipe(new Do<T>(select));
    }

    public static IFuture<T> Do<T>(this IFuture<T> future, Func<T, Task> select)
    {
        return future.Pipe(new Do<T>(select));
    }
}