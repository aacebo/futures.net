namespace Futures;

public partial class Future<T, TOut> : IStream<T, TOut>
{
    public TOut Next(T value)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Started;
        var output = _resolve(value);
        Value = output;

        foreach (var (_, consumer) in _consumers)
        {
            consumer.Next(output);
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

        foreach (var (_, consumer) in _consumers)
        {
            consumer.Complete();
        }

        return Value;
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

    public IStream<T, TNext> Map<TNext>(Func<TOut, TNext> next)
    {
        return new Future<T, TNext>(v => next(Next(v)), Token);
    }

    public IStream<T, TNext> Map<TNext>(Func<TOut, Task<TNext>> next)
    {
        return new Future<T, TNext>(v => next(Next(v)).ConfigureAwait(false).GetAwaiter().GetResult(), Token);
    }

    public IStream<T, TNext> Map<TNext>(IStream<TOut, TNext> transform)
    {
        return new Future<T, TNext>(v => transform.Next(Next(v)), Token);
    }
}

public partial class Future<T1, T2, TOut> : IStream<T1, T2, TOut>
{
    public TOut Next(T1 a, T2 b)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Started;
        var output = _resolve(a, b);
        Value = output;

        foreach (var (_, consumer) in _consumers)
        {
            consumer.Next(output);
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

        foreach (var (_, consumer) in _consumers)
        {
            consumer.Complete();
        }

        return Value;
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

    public IStream<T1, T2, TNext> Map<TNext>(Func<TOut, TNext> next)
    {
        return new Future<T1, T2, TNext>((a, b) => next(Next(a, b)), Token);
    }

    public IStream<T1, T2, TNext> Map<TNext>(Func<TOut, Task<TNext>> next)
    {
        return new Future<T1, T2, TNext>((a, b) => next(Next(a, b)).ConfigureAwait(false).GetAwaiter().GetResult(), Token);
    }

    public IStream<T1, T2, TNext> Map<TNext>(IStream<TOut, TNext> transform)
    {
        return new Future<T1, T2, TNext>((a, b) => transform.Next(Next(a, b)), Token);
    }
}