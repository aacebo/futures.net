using System.Collections;

namespace Futures;

public partial class Future<T> : IEnumerable<T>, IAsyncEnumerable<T>
{
    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator<T>(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new Enumerator<T>(this);
    }
}

public static class IEnumerableExtensions
{
    public static Future<T> ToFuture<T>(this IEnumerable<T> enumerable)
    {
        var future = new Future<IEnumerable<T>, T>((items, producer) =>
        {
            foreach (var value in items)
            {
                producer.Next(value);
            }

            producer.Complete();
        });

        future.NextAsync(enumerable);
        return future;
    }
}

public static class IAsyncEnumerableExtensions
{
    public static Future<T> ToFuture<T>(this IAsyncEnumerable<T> enumerable)
    {
        return new Future<T>(async (_, producer) =>
        {
            await foreach (var value in enumerable)
            {
                producer.Next(value);
            }

            producer.Complete();
        });
    }
}

internal partial class Enumerator<T> : IEnumerator<T>, IAsyncEnumerator<T>
{
    public T Current => _values[_index];
    object IEnumerator.Current => Current!;

    protected int _index = -1;
    protected bool _complete = false;
    protected List<T> _values;
    protected Subscription _subscription;

    public Enumerator(Future<T> future)
    {
        _values = [];
        _subscription = future.Subscribe(new()
        {
            OnNext = _values.Add,
            OnComplete = () => _complete = true,
            OnError = (_) => _complete = true,
            OnCancel = () => _complete = true
        });
    }

    ~Enumerator()
    {
        Dispose();
        DisposeAsync();
    }

    public void Dispose()
    {
        _subscription.Dispose();
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        return default;
    }

    public bool MoveNext()
    {
        _index++;

        while (_index >= _values.Count && !_complete)
        {
            Task.Delay(100).Wait();
        }

        return _index < _values.Count;
    }

    public async ValueTask<bool> MoveNextAsync()
    {
        _index++;

        while (_index >= _values.Count && !_complete)
        {
            await Task.Delay(100);
        }

        return _index < _values.Count;
    }

    public void Reset()
    {
        throw new NotSupportedException();
    }
}