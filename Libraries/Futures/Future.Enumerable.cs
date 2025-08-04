namespace Futures;

public partial class Future<TIn, TOut>
{
    public IAsyncEnumerator<TOut> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new FutureEnumerator<TIn, TOut>(this);
    }
}

internal class FutureEnumerator<TIn, TOut> : IAsyncEnumerator<TOut>
{
    public TOut Current => _values[_index];

    private int _index = -1;
    private bool _complete = false;
    private readonly List<TOut> _values;
    private readonly Subscription _subscription;

    public FutureEnumerator(IFuture<TIn, TOut> future)
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
        DisposeAsync();
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

    public ValueTask DisposeAsync()
    {
        _subscription.Dispose();
        GC.SuppressFinalize(this);
        return default;
    }
}