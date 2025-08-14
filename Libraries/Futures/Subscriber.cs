namespace Futures;

public class Subscriber<T, TOut> : Subscription, IConsumer<T, TOut>, IDisposable
{
    public Func<T, IEnumerable<TOut>>? Next { get; set; }
    public Action? Complete { get; set; }
    public Action<Exception>? Error { get; set; }
    public Action? Cancel { get; set; }

    public Subscriber() : base()
    {

    }

    public Subscriber(Future<T, TOut> destination) : base()
    {
        Next = destination.Next;
        Complete = () => destination.Complete();
        Error = destination.Error;
        Cancel = destination.Cancel;
    }

    public Subscriber(IConsumer<T, TOut> destination) : base()
    {
        Next = destination.OnNext;
        Complete = destination.OnComplete;
        Error = destination.OnError;
        Cancel = destination.OnCancel;
    }

    ~Subscriber()
    {
        Dispose();
    }

    public IEnumerable<TOut> OnNext(T value)
    {
        _count++;

        if (_limit != null && _count >= _limit)
        {
            UnSubscribe();
        }

        if (Next is not null)
        {
            return Next(value);
        }

        return [];
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
}