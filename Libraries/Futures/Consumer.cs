namespace Futures;

/// <summary>
/// consumes/reads data from some Future
/// </summary>
public interface IConsumer
{
    public void UnSubscribe();
}

/// <summary>
/// consumes/reads data from some Future
/// </summary>
/// <typeparam name="T">the type of data consumed</typeparam>
public class Consumer<T> : IConsumer
{
    public Action<T>? OnNext { get; set; }
    public Action? OnComplete { get; set; }
    public Action<Exception>? OnError { get; set; }
    public Action? OnCancel { get; set; }

    internal Subscription Subscription;

    public Consumer()
    {
        Subscription = new(() => { });
    }

    internal Consumer(Subscription subscription)
    {
        Subscription = subscription;
    }

    ~Consumer()
    {
        Dispose();
    }

    public void Dispose()
    {
        Subscription.Dispose();
        GC.SuppressFinalize(this);
    }

    public void UnSubscribe()
    {
        Subscription.UnSubscribe();
    }

    internal void Next(T value)
    {
        if (OnNext is null)
        {
            return;
        }

        OnNext(value);
    }

    internal void Complete()
    {
        if (OnComplete is null)
        {
            return;
        }

        OnComplete();
    }

    internal void Error(Exception error)
    {
        if (OnError is null)
        {
            return;
        }

        OnError(error);
    }

    internal void Cancel()
    {
        if (OnCancel is null)
        {
            return;
        }

        OnCancel();
    }
}