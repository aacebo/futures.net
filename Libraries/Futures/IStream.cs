namespace Futures;

public interface IStream<in T, TOut> : IConsumer<T>, IProducer<TOut>
{
    TOut Next(T value);
}

public interface IStream<T1, T2, TOut> : IConsumer<T1, T2>, IProducer<TOut>
{
    TOut Next(T1 a, T2 b);
}