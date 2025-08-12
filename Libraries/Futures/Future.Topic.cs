namespace Futures;

public partial class Future<T> : ITopic<T>
{
    public IStream<T, TNext> Map<TNext>(Func<T, TNext> next)
    {
        return new Future<T, TNext>(v => next(v));
    }

    public IStream<T, TNext> Map<TNext>(Func<T, Task<TNext>> next)
    {
        return new Future<T, TNext>(v => next(v).ConfigureAwait(false).GetAwaiter().GetResult());
    }

    public IStream<T, TNext> Map<TNext>(IStream<T, TNext> stream)
    {
        return new Future<T, TNext>(stream.Next);
    }
}