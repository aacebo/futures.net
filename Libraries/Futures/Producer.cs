namespace Futures;

/// <summary>
/// publishes/writes data to some Future
/// </summary>
/// <typeparam name="T">the type of data published</typeparam>
public class Producer<T>
{
    private readonly Future<T> _destination;

    internal Producer(Future<T> destination)
    {
        _destination = destination;
    }

    ~Producer()
    {
        Dispose();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public void Next(T value)
    {
        _destination.Next(value);
    }

    public Task NextAsync(T value)
    {
        return _destination.NextAsync(value);
    }

    public void Complete()
    {
        _destination.Complete();
    }

    public Task CompleteAsync()
    {
        return _destination.CompleteAsync();
    }

    public void Error(Exception error)
    {
        _destination.Error(error);
    }

    public Task ErrorAsync(Exception error)
    {
        return _destination.ErrorAsync(error);
    }

    public void Cancel()
    {
        _destination.Cancel();
    }

    public Task CancelAsync()
    {
        return _destination.CancelAsync();
    }
}