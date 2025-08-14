namespace Futures.Operators;

public sealed class Merge<T>(IFuture<T> other) : IOperator<T, T>
{
    public IFuture<T> Invoke(IFuture<T> source)
    {
        return new Future<T>(destination =>
        {
            other.Subscribe(destination);
            source.Subscribe(destination);
        });
    }
}

public static partial class FutureExtensions
{
    public static IFuture<T> Merge<T>(this IFuture<T> future, IFuture<T> other)
    {
        return future.Pipe(new Merge<T>(other));
    }
}