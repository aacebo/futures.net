namespace Futures;

/// <summary>
/// consumes/reads data from some Future
/// </summary>
/// <typeparam name="T">the type of data consumed</typeparam>
public class Subscriber<T> : Subscription<T>, IConsumer<T>, IDisposable
{
    public Action<T>? OnNext { get; set; }
    public Action? OnComplete { get; set; }
    public Action<Exception>? OnError { get; set; }
    public Action? OnCancel { get; set; }

    public Subscriber() : base()
    {

    }

    public Subscriber(IConsumer<T> destination) : base()
    {
        OnNext = destination.Next;
        OnComplete = destination.Complete;
        OnError = destination.Error;
        OnCancel = destination.Cancel;
    }

    ~Subscriber()
    {
        Dispose();
    }

    public void Next(T value)
    {
        _count++;

        if (OnNext is not null)
        {
            OnNext(value);
        }

        if (_limit != null && _count >= _limit)
        {
            UnSubscribe();
        }
    }

    public void Complete()
    {
        if (OnComplete is not null)
        {
            OnComplete();
        }
    }

    public void Error(Exception error)
    {
        if (OnError is not null)
        {
            OnError(error);
        }
    }

    public void Cancel()
    {
        if (OnCancel is not null)
        {
            OnCancel();
        }
    }

    public static Subscriber<T> From<TIn>(Future<T> future)
    {
        return new Subscriber<T>(future);
    }

    public static Subscriber<T> From<TIn>(Future<TIn, T> future)
    {
        return new Subscriber<T>(new Future<T>(destination =>
        {
            future.Subscribe(new Subscriber<T>(destination));
        }));
    }

    public static Subscriber<T> From<T1, T2>(Future<T1, T2, T> future)
    {
        return new Subscriber<T>(new Future<T>(destination =>
        {
            future.Subscribe(new Subscriber<T>(destination));
        }));
    }
}
