namespace Futures;

public static partial class Future
{
    public static Future<object?> Run(Action<Future<object?>> onInit, CancellationToken cancellation = default)
    {
        var future = new Future<object?>(cancellation);
        onInit(future);
        return future;
    }

    public static Future<T> Run<T>(Action<Future<T>> onInit, CancellationToken cancellation = default)
    {
        var future = new Future<T>(cancellation);
        onInit(future);
        return future;
    }

    public static Future<T> Run<T>(Func<Future<T>, Task> onInit, CancellationToken cancellation = default)
    {
        var future = new Future<T>(cancellation);
        onInit(future).ConfigureAwait(false);
        return future;
    }
}