namespace Futures;

/// <summary>
/// a future that streams bi-directional input/output
/// </summary>
/// <typeparam name="T">the input data type</typeparam>
/// <typeparam name="TOut">the output data type</typeparam>
public interface IStream<in T, TOut> : IFuture<TOut>
{
    IEnumerable<TOut> Next(params T[] values);
    IEnumerable<TOut> Next(IEnumerable<T> values);
    TOut Complete();
    void Error(Exception ex);
    void Cancel();

    IStream<T, TNext> Map<TNext>(Func<TOut, TNext> next);
    IStream<T, TNext> Map<TNext>(Func<TOut, Task<TNext>> next);
    IStream<T, TNext> Map<TNext>(ITransformer<TOut, TNext> next);
    IStream<T, TNext> Map<TNext>(IStream<TOut, TNext> next);

    public delegate void Resolver(T value, IConsumer<TOut> consumer);
}