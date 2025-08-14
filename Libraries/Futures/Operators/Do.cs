namespace Futures.Operators;

public sealed class Do<T>(Fn<T> selector) : IOperator<T, T>
{
    public IFuture<T> Invoke(IFuture<T> source)
    {
        return new Future<T>(destination =>
        {
            source.Subscribe(new Subscriber<T>(destination)
            {
                OnNext = value =>
                {
                    selector.Resolve(value);
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