namespace Futures;

/// <summary>
/// publishes/writes data to some Future
/// </summary>
/// <typeparam name="T">the type of data published</typeparam>
public class Producer<T> : IProducer<T>
{
    private readonly IConsumer<T> _consumer;

    internal Producer(IConsumer<T> destination)
    {
        _consumer = destination;
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
        _consumer.OnNext(value);
    }

    public Task NextAsync(T value)
    {
        _consumer.OnNext(value);
        return Task.CompletedTask;
    }

    public void Complete()
    {
        _consumer.OnComplete();
    }

    public Task CompleteAsync()
    {
        _consumer.OnComplete();
        return Task.CompletedTask;
    }

    public void Error(Exception error)
    {
        _consumer.OnError(error);
    }

    public Task ErrorAsync(Exception error)
    {
        _consumer.OnError(error);
        return Task.CompletedTask;
    }

    public void Cancel()
    {
        _consumer.OnCancel();
    }

    public Task CancelAsync()
    {
        _consumer.OnCancel();
        return Task.CompletedTask;
    }
}