namespace Futures;

public class Subscriber<T> : Subscription, IConsumer<T>, IDisposable
{
    public Action<T>? Next { get; set; }
    public Action? Complete { get; set; }
    public Action<Exception>? Error { get; set; }
    public Action? Cancel { get; set; }

    internal Guid Id { get; } = Guid.NewGuid();

    public Subscriber() : base()
    {

    }

    public Subscriber(IConsumer<T> destination) : base()
    {
        Next = destination.OnNext;
        Complete = destination.OnComplete;
        Error = destination.OnError;
        Cancel = destination.OnCancel;
    }

    public Subscriber(Future<T> future) : base()
    {
        Next = v => future.Next(v);
    }

    ~Subscriber()
    {
        Dispose();
    }

    public void OnNext(T value)
    {
        _count++;

        if (_limit != null && _count >= _limit)
        {
            UnSubscribe();
        }

        if (Next is not null)
        {
            Next(value);
        }
    }

    public void OnComplete()
    {
        if (Complete is not null)
        {
            Complete();
        }
    }

    public void OnError(Exception error)
    {
        if (Error is not null)
        {
            Error(error);
        }
    }

    public void OnCancel()
    {
        if (Cancel is not null)
        {
            Cancel();
        }
    }

    public static Subscriber<T> From<TOut>(Future<T, TOut> future)
    {
        return new()
        {
            Next = v => future.Next(v),
            Complete = () => future.Complete(),
            Error = future.Error,
            Cancel = future.Cancel,
        };
    }
}