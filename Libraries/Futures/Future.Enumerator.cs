using System.Collections;

namespace Futures;

internal partial class Enumerator<T> : IEnumerator<T>, IAsyncEnumerator<T>
{
    public T Current => _values.Dequeue();
    object IEnumerator.Current => Current!;

    protected bool _complete = false;
    protected Queue<T> _values;
    protected Subscription _subscription;

    public Enumerator(FutureBase<T> future)
    {
        _values = new(future.AsList());
        _complete = future.IsComplete;
        _subscription = future.Subscribe(new Consumer<T>()
        {
            OnNext = _values.Enqueue,
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
        while (_values.Count == 0 && !_complete)
        {
            Task.Delay(100).Wait();
        }

        return _values.Count > 0;
    }

    public async ValueTask<bool> MoveNextAsync()
    {
        while (_values.Count == 0 && !_complete)
        {
            await Task.Delay(100);
        }

        return _values.Count > 0;
    }

    public void Reset()
    {
        throw new NotSupportedException();
    }
}