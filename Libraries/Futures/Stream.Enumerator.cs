using System.Collections;

namespace Futures;

internal partial class Enumerator<T> : IEnumerator<T>, IAsyncEnumerator<T>
{
    public T Current => _values.Dequeue();
    object IEnumerator.Current => Current!;

    protected bool _complete = false;
    protected Queue<T> _values;
    protected ISubscription _subscription;

    public Enumerator(IProducer<T> producer)
    {
        _values = [];
        _complete = false;
        _subscription = producer.Subscribe(new Subscriber<T>()
        {
            Next = (_, value) => _values.Enqueue(value),
            Complete = () => _complete = true,
            Error = (_) => _complete = true,
            Cancel = () => _complete = true
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