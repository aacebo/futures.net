namespace Futures.Operators;

public sealed class Catch<T> : IOperator<T>
{
    private readonly Func<Exception, IFuture<T>, T> _selector;

    public Catch(Func<Exception, IFuture<T>, T> select)
    {
        _selector = select;
    }

    public Catch(Func<Exception, IFuture<T>, Task<T>> select)
    {
        _selector = (err, caught) => select(err, caught).ConfigureAwait(false).GetAwaiter().GetResult();
    }

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
                    res = Future<T>.From(_selector(err, Invoke(source)));

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