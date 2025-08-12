namespace Futures;

public partial class Future<T> : ITopic<T>
{
    public ITransformer<T, TNext> Map<TNext>(Func<T, TNext> next)
    {
        return new Future<T, TNext>(v => next(v));
    }

    public ITransformer<T, TNext> Map<TNext>(Func<T, Task<TNext>> next)
    {
        return new Future<T, TNext>(v => next(v).ConfigureAwait(false).GetAwaiter().GetResult());
    }

    public ITransformer<T, TNext> Map<TNext>(ITransformer<T, TNext> next)
    {
        return new Future<T, TNext>(next.Next);
    }
}