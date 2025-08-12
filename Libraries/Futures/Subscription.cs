namespace Futures;

public interface ISubscription : IDisposable
{
    public bool IsOpen { get; }
    public void UnSubscribe();
    public ISubscription Limit(int limit);
    public ISubscription Timeout(TimeSpan after);
}

public class Subscription<T> : ISubscription
{
    public bool IsOpen { get; private set; }

    private readonly Action _unsubscribe;
    private int? _limit;
    private int _count = 0;

    internal Subscription()
    {
        _unsubscribe = () => { };
        IsOpen = true;
    }

    internal Subscription(Action unsubscribe)
    {
        _unsubscribe = unsubscribe;
        IsOpen = true;
    }

    ~Subscription()
    {
        Dispose();
    }

    public void Dispose()
    {
        UnSubscribe();
        GC.SuppressFinalize(this);
    }

    public void UnSubscribe()
    {
        if (!IsOpen) return;

        _unsubscribe();
        IsOpen = false;
    }

    public ISubscription Limit(int limit)
    {
        _limit = limit;
        return this;
    }

    public ISubscription Timeout(TimeSpan after)
    {
        Task.Delay(after).ContinueWith(_ => UnSubscribe());
        return this;
    }

    internal void OnNext()
    {
        _count++;

        if (_limit != null && _count >= _limit)
        {
            UnSubscribe();
        }
    }
}