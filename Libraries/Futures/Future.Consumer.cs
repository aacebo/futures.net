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
        Last = value;

        foreach (var (_, subscriber) in _consumers)
        {
            subscriber.Next(value);
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

        foreach (var (_, subscriber) in _consumers)
        {
            subscriber.Complete();
        }
    }
}

public partial class Future<T, TOut> : IConsumer<T, TOut>
{
    public TOut Next(T value)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Started;
        var output = _selector(value);
        Last = output;

        foreach (var (_, subscriber) in _consumers)
        {
            subscriber.Next(output);
        }

        return output;
    }

    public TOut Complete()
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

        foreach (var (_, subscriber) in _consumers)
        {
            subscriber.Complete();
        }

        return Value;
    }
}

public partial class Future<T1, T2, TOut> : IConsumer<T1, T2, TOut>
{
    public TOut Next(T1 a, T2 b)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Started;
        var output = _selector(a, b);
        Last = output;

        foreach (var (_, subscriber) in _consumers)
        {
            subscriber.Next(output);
        }

        return output;
    }

    public TOut Complete()
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

        foreach (var (_, subscriber) in _consumers)
        {
            subscriber.Complete();
        }

        return Value;
    }
}