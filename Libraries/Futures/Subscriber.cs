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
}

public class Subscriber<T, TOut> : Subscription<T>, IConsumer<T, TOut>, IDisposable
{
    public required Func<T, TOut> OnNext { get; set; }
    public required Func<TOut> OnComplete { get; set; }
    public Action<Exception>? OnError { get; set; }
    public Action? OnCancel { get; set; }

    public Subscriber() : base()
    {

    }

    public Subscriber(IConsumer<T, TOut> destination) : base()
    {
        OnNext = v => destination.Next(v);
        OnComplete = () => destination.Complete();
        OnError = destination.Error;
        OnCancel = destination.Cancel;
    }

    ~Subscriber()
    {
        Dispose();
    }

    public TOut Next(T value, Action<TOut>? result = null)
    {
        _count++;
        var @out = OnNext(value);

        if (_limit != null && _count >= _limit)
        {
            UnSubscribe();
        }

        return @out;
    }

    public TOut Complete(Action<TOut>? result = null)
    {
        return OnComplete();
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
}