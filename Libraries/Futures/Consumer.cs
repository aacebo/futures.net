namespace Futures;

/// <summary>
/// consumes/reads data from some Future
/// </summary>
/// <typeparam name="T">the type of data consumed</typeparam>
public class Consumer<T> : IConsumer<T>, IDisposable
{
    public Action<T>? OnNext { get; set; }
    public Action? OnComplete { get; set; }
    public Action<Exception>? OnError { get; set; }
    public Action? OnCancel { get; set; }

    public Consumer()
    {

    }

    public Consumer(IFuture<T> future)
    {
        OnNext = future.Next;
        OnComplete = future.Complete;
        OnError = future.Error;
        OnCancel = future.Cancel;
    }

    ~Consumer()
    {
        Dispose();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public void Next(T value)
    {
        if (OnNext is not null)
        {
            OnNext(value);
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
