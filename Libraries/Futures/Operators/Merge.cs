namespace Futures.Operators;

public sealed class Merge<T> : IOperator<T>
{
    private readonly IFuture<T> _other;

    public Merge(IFuture<T> other)
    {
        _other = other;
    }

    public IFuture<T> Invoke(IFuture<T> source)
    {
        return new Future<T>(destination =>
        {
            _other.Subscribe(destination);
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