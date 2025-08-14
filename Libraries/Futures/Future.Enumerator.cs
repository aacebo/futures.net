using System.Collections;

namespace Futures;

internal partial class Enumerator<T, TOut> : IEnumerator<TOut>, IAsyncEnumerator<TOut>
{
    public TOut Current => _values.Dequeue();
    object IEnumerator.Current => Current!;

    protected bool _complete = false;
    protected Queue<TOut> _values;
    protected ISubscription _subscription;

    public Enumerator(IProducer<T, TOut> producer)
    {
        _values = [];
        _complete = false;
        _subscription = producer.Subscribe(new Subscriber<TOut, TOut>()
        {
            Next = v =>
            {
                _values.Enqueue(v);
                return [];
            },
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