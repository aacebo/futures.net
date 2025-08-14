namespace Futures.Operators;

public sealed class Catch<T>(Fn<Exception, IFuture<T>, T> selector) : IOperator<T, T>
{
    public IFuture<T> Invoke(IFuture<T> source)
    {
        return new Future<T>(destination =>
        {
            var sync = false;
            ISubscription? subscription = null;
            Future<T>? res = null;

            subscription = source.Subscribe(new Subscriber<T>(destination)
            {
                OnError = err =>
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

public sealed class Catch<T, TOut>(Fn<Exception, IFuture<T>, T> selector) : IOperator<T, TOut>
{
    public IFuture<T, TOut> Invoke(IFuture<T, TOut> source)
    {
        return new Future<T, TOut>(destination =>
        {
            var sync = false;
            ISubscription? subscription = null;
            Future<T>? res = null;

            subscription = source.Subscribe(new Subscriber<T>(destination)
            {
                OnError = err =>
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