namespace Futures;

public static partial class Future
{
    public static Future<T> FromValue<T>(T value)
    {
        var future = new Future<T>();
        future.Next(value);
        future.Complete();
        return future;
    }

    public static Future<T> FromError<T>(Exception error)
    {
        var future = new Future<T>();
        future.Error(error);
        return future;
    }

    public static Future<T> From<T>(IEnumerable<T> enumerable)
    {
        return Run<T>(future =>
        {
            foreach (var item in enumerable)
            {
                future.Next(item);
            }

            future.Complete();
        });
    }

    public static Future<T> From<T>(IAsyncEnumerable<T> enumerable)
    {
        return Run<T>(async future =>
        {
            await foreach (var item in enumerable)
            {
                future.Next(item);
            }

            future.Complete();
        });
    }
}