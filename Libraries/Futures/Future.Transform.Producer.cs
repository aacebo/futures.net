namespace Futures;

public partial class Future<T, TOut> : IProducer<TOut>
{
    public ISubscription Subscribe(IConsumer<TOut> consumer)
    {
        return Out.Subscribe(consumer);
    }

    public ISubscription Subscribe(Future<TOut> future)
    {
        return Out.Subscribe(future);
    }

    public ISubscription Subscribe<TNext>(Future<TOut, TNext> future)
    {
        return Out.Subscribe(future);
    }

    public ISubscription Subscribe(Action<object, TOut>? next = null, Action? complete = null, Action<Exception>? error = null, Action? cancel = null)
    {
        return Out.Subscribe(next, complete, error, cancel);
    }

    public ISubscription Subscribe(Func<object, TOut, Task>? next = null, Action? complete = null, Action<Exception>? error = null, Action? cancel = null)
    {
        return Out.Subscribe(next, complete, error, cancel);
    }
}

public partial class Future<T1, T2, TOut> : IProducer<TOut>
{
    public ISubscription Subscribe(IConsumer<TOut> consumer)
    {
        return Out.Subscribe(consumer);
    }

    public ISubscription Subscribe(Future<TOut> future)
    {
        return Out.Subscribe(future);
    }

    public ISubscription Subscribe<TNext>(Future<TOut, TNext> future)
    {
        return Out.Subscribe(future);
    }

    public ISubscription Subscribe(Action<object, TOut>? next = null, Action? complete = null, Action<Exception>? error = null, Action? cancel = null)
    {
        return Out.Subscribe(next, complete, error, cancel);
    }

    public ISubscription Subscribe(Func<object, TOut, Task>? next = null, Action? complete = null, Action<Exception>? error = null, Action? cancel = null)
    {
        return Out.Subscribe(next, complete, error, cancel);
    }
}