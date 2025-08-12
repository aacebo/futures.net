using System.Collections.Concurrent;

namespace Futures.Collections;

public partial class Stream<T, TOut> : FutureBase<TOut>, IFuture<TOut>, IStream<T, TOut>
{
    private readonly ConcurrentQueue<TOut> _queue = new();
    private readonly Func<IEnumerable<T>, IEnumerable<TOut>> _resolve;

    public Stream(Func<IEnumerable<T>, IEnumerable<TOut>> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolve = resolve;
    }

    public Stream(Func<IEnumerable<T>, TOut> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolve = values => [resolve(values)];
    }

    public Stream(Func<IEnumerable<T>, Task<TOut>> resolve, CancellationToken cancellation = default) : base(cancellation)
    {
        _resolve = values => [resolve(values).ConfigureAwait(false).GetAwaiter().GetResult()];
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

        foreach (var value in _resolve(values))
        {
            _queue.Enqueue(value);
            Value = value;
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