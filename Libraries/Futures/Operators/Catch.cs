namespace Futures.Operators;

public sealed class Catch<T, TOut>(Fn<Exception, Future<T, TOut>, T> selector) : IOperator<T, TOut>
{
    public Future<T, TOut> Invoke(Future<T, TOut> source)
    {
        return new Future<T, TOut>(destination =>
        {
            var sync = false;
            ISubscription? subscription = null;
            Future<T>? res = null;

            subscription = source.Subscribe(new Subscriber<T, TOut>(destination)
            {
                Error = err =>
                {
                    res = Future<T>.From(selector.Resolve(err, Invoke(source)));

                    if (subscription is not null)
                    {
                        subscription.UnSubscribe();
                        subscription = null;
                        res.Subscribe(destination);
                    }
                    else
                    {
                        sync = true;
                    }
                }
            });

            if (sync)
            {
                subscription.UnSubscribe();
                subscription = null;
                res?.Subscribe(destination);
            }
        });
    }
}

public static partial class FutureExtensions
{
    public static IFuture<T> Catch<T>(this IFuture<T> future, Func<Exception, IFuture<T>, T> select)
    {
        return future.Pipe(new Catch<T>(select));
    }

    public static IFuture<T> Catch<T>(this IFuture<T> future, Func<Exception, IFuture<T>, Task<T>> select)
    {
        return future.Pipe(new Catch<T>(select));
    }
}