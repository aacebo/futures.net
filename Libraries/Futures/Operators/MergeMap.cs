namespace Futures.Operators;

public sealed class MergeMap<T, TNext>(Fn<T, Future<TNext>> selector) : ITransformer<T, TNext>
{
    public Future<T, TNext> Invoke(Future<T> src)
    {
        return Future<T>
            .Run(dest => src.Subscribe(dest))
            .Map(selector.Invoke);
    }
}