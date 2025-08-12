
namespace Futures;

public partial class Future<T> : IConsumer<T>
{
    public void Next(T value)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Started;
        value = _resolve(value);
        Value = value;

        foreach (var (_, consumer) in _consumers)
        {
            consumer.Next(value);
        }
    }

    public void Complete()
    {
        if (Value is null)
        {
            throw new InvalidOperationException("attempted to complete a future with no data");
        }

        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Success;
        _source.TrySetResult(Value);

        foreach (var (_, consumer) in _consumers)
        {
            consumer.Complete();
        }
    }

    public void Error(Exception ex)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Error;
        _source.TrySetException(ex);

        foreach (var (_, consumer) in _consumers)
        {
            consumer.Error(ex);
        }
    }

    public void Cancel()
    {
        if (IsComplete) return;

        State = State.Cancelled;
        _source.TrySetCanceled();

        foreach (var (_, consumer) in _consumers)
        {
            consumer.Cancel();
        }
    }
}