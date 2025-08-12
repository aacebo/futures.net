namespace Futures.Operators;

public sealed class Catch<T> : IOperator<T>
{
    private readonly Func<Exception, IFuture<T>, T> _next;

    public Catch(Func<Exception, IFuture<T>, T> next)
    {
        _next = next;
    }

    public Catch(Func<Exception, IFuture<T>, Task<T>> next)
    {
        _next = (err, caught) => next(err, caught).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public IFuture<T> Invoke(IFuture<T> source)
    {
        return new Future<T>((destination, consumer) =>
        {
            var sync = false;
            ISubscription? subscription = null;
            Future<T>? res = null;

            subscription = source.Subscribe(new Consumer<T>(destination)
            {
                OnError = err =>
                {
                    res = Future<T>.From(_next(err, Invoke(source)));

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
        }, source.Token);
    }
}