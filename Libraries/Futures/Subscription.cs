namespace Futures;

public class Subscription
{
    public bool IsOpen { get; private set; }

    protected readonly Action _unsubscribe;

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
}