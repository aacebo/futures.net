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

    protected int? _limit;
    protected int _count = 0;
    private readonly List<Action> _teardown;

    internal Subscription(params Action[] teardown)
    {
        IsOpen = true;
        _teardown = [.. teardown];
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
        IsOpen = false;

        foreach (var teardown in _teardown)
        {
            teardown();
        }
    }

    public void AddTearDown(Action action)
    {
        _teardown.Add(action);
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
}