namespace Futures.Operators;

public sealed class Retry<T> : IOperator<T>
{
    private readonly int _attempts;
    private readonly int _delay;

    public Retry(int attempts = 3, int delay = 0)
    {
        _attempts = attempts;
        _delay = delay;
    }

    public IFuture<T> Invoke(IFuture<T> source)
    {
        return new Future<T>(destination =>
        {
            var i = 0;
            ISubscription? subscription = null;

            void Retry()
            {
                var sync = false;
                void ReSubscribe()
                {
                    if (subscription is null)
                    {
                        sync = true;
                        return;
                    }

                    subscription.UnSubscribe();
                    subscription = null;
                    Retry();
                }

                subscription = source.Subscribe(new Subscriber<T>(destination)
                {
                    OnNext = value =>
                    {
                        i = 0;
                        destination.Next(value);
                    },
                    OnError = err =>
                    {
                        i++;

                        if (i >= _attempts)
                        {
                            destination.Error(err);
                            return;
                        }

                        if (_delay > 0)
                        {
                            Task.Delay(_delay).Wait();
                        }

                        ReSubscribe();
                    }
                });

                if (sync)
                {
                    ReSubscribe();
                }
            }

            Retry();
        });
    }
}

public static partial class FutureExtensions
{
    public static IFuture<T> Retry<T>(this IFuture<T> future, int attempts = 3, int delay = 0)
    {
        return future.Pipe(new Retry<T>(attempts, delay));
    }
}