namespace Futures.Collections;

public partial class Stream<T, TOut> : FutureBase<TOut>, IFuture<TOut>, IStream<T, TOut>
{
    private readonly Func<IEnumerable<T>, IEnumerable<TOut>> _resolve;

    public Stream(Func<IEnumerable<T>, IEnumerable<TOut>> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolve = resolve;
    }

    public IEnumerable<TOut> Next(params T[] values)
    {
        return Next((IEnumerable<T>)values);
    }

    public IEnumerable<TOut> Next(IEnumerable<T> values)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Started;
        var output = _resolve(values);

        foreach (var value in _resolve(values))
        {
            Value = value;

            foreach (var (_, consumer) in _consumers)
            {
                consumer.Next(value);
            }

            yield return value;
        }
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
        return new Stream<T, TNext>(v =>
        {
            List<TNext> outputs = [];

            foreach (var value in Next(v))
            {
                outputs.Add(next(value));
            }

            return outputs;
        }, Token);
    }

    public IStream<T, TNext> Map<TNext>(Func<TOut, Task<TNext>> next)
    {
        return new Stream<T, TNext>(v =>
        {
            List<TNext> outputs = [];

            foreach (var value in Next(v))
            {
                outputs.Add(next(value).ConfigureAwait(false).GetAwaiter().GetResult());
            }

            return outputs;
        }, Token);
    }

    public IStream<T, TNext> Map<TNext>(ITransformer<TOut, TNext> next)
    {
        return new Stream<T, TNext>(v =>
        {
            List<TNext> outputs = [];

            foreach (var value in Next(v))
            {
                outputs.Add(next.Next(value));
            }

            return outputs;
        }, Token);
    }

    public IStream<T, TNext> Map<TNext>(IStream<TOut, TNext> next)
    {
        return new Stream<T, TNext>(v => next.Next(Next(v)), Token);
    }
}