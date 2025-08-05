namespace Futures;

/// <summary>
/// a future that takes the same input/output
/// </summary>
/// <typeparam name="T">the output type</typeparam>
public interface IFuture<T> : IFuture<T, T>;

/// <summary>
/// a future that takes the same input/output
/// </summary>
/// <typeparam name="T">the output type</typeparam>
public partial class Future<T> : Future<T, T>, IFuture<T>, IFuture<T, T>
{
    public Future(CancellationToken cancellation = default) : base(value => value, cancellation)
    {

    }

    public Future(T value, CancellationToken cancellation = default) : base(value => value, cancellation)
    {
        Next(value);
    }

    public Future(Func<T, T> resolve, CancellationToken cancellation = default) : base(resolve, cancellation)
    {

    }

    public static Future<T> From(T value)
    {
        var future = new Future<T>(value);
        future.Complete();
        return future;
    }

    public static Future<T> From(Exception ex)
    {
        var future = new Future<T>();
        future.Error(ex);
        return future;
    }

    public static Future<T> From(IEnumerable<T> enmerable, CancellationToken cancellation = default)
    {
        var future = new Future<T>(cancellation);

        _ = Task.Run(() =>
        {
            foreach (var item in enmerable)
            {
                future.Next(item);
            }

            future.Complete();
        }, future.Token);

        return future;
    }

    public static Future<T> From(IAsyncEnumerable<T> enmerable, CancellationToken cancellation = default)
    {
        var future = new Future<T>(cancellation);

        _ = Task.Run(async () =>
        {
            await foreach (var item in enmerable)
            {
                future.Next(item);
            }

            future.Complete();
        }, future.Token);

        return future;
    }

    public static implicit operator ReadOnlyFuture<T, T>(Future<T> future) => new(future);
}