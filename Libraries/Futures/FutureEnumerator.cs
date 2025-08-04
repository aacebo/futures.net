using System.Collections;

namespace Futures;

internal partial class FutureEnumerator<T> : IEnumerator<T>, IAsyncEnumerator<T>
{
    public T Current => _values[_index];
    object IEnumerator.Current => Current!;

    protected int _index = -1;
    protected bool _complete = false;
    protected List<T> _values;
    protected Subscription _subscription;

    public FutureEnumerator(FutureBase<T> future)
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

    ~FutureEnumerator()
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