namespace Futures;

public interface ISubscribable<T>
{
    public Subscription Subscribe(Subscriber<T> subscriber);
}

public class Subscription(Action unsubscribe) : IDisposable
{
    ~Subscription()
    {
        Dispose();
    }

    public void Dispose()
    {
        unsubscribe();
        GC.SuppressFinalize(this);
    }
}

public class Subscriber<T> : IDisposable
{
    public Action<T>? OnNext { get; set; }
    public Action? OnComplete { get; set; }
    public Action<Exception>? OnError { get; set; }
    public Action? OnCancel { get; set; }

    ~Subscriber()
    {
        Dispose();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public void Next(T value)
    {
        if (OnNext is null)
        {
            return;
        }

        OnNext(value);
    }

    public void Complete()
    {
        if (OnComplete is null)
        {
            return;
        }

        OnComplete();
    }

    public void Error(Exception error)
    {
        if (OnError is null)
        {
            return;
        }

        OnError(error);
    }

    public void Cancel()
    {
        if (OnCancel is null)
        {
            return;
        }

        OnCancel();
    }
}