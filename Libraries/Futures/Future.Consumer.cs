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
    public void Next(TOut value)
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

    public TOut Next(T value, Action<TOut>? result = null)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Started;
        var output = _selector.Invoke(value);
        Last = output;

        foreach (var (_, subscriber) in _consumers)
        {
            subscriber.Next(output);
        }

        if (result is not null) result(output);
        return output;
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

    public TOut Complete(Action<TOut>? result = null)
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

        if (result is not null) result(Value);
        return Value;
    }
}

public partial class Future<T1, T2, TOut> : IConsumer<T1, T2, TOut>
{
    public void Next(TOut value)
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

    public TOut Next(T1 a, T2 b, Action<TOut>? result = null)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Started;
        var output = _selector.Invoke(a, b);
        Last = output;

        foreach (var (_, subscriber) in _consumers)
        {
            subscriber.Next(output);
        }

        if (result is not null) result(output);
        return output;
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

    public TOut Complete(Action<TOut>? result = null)
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

        if (result is not null) result(Value);
        return Value;
    }
}