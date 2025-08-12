namespace Futures;

/// <summary>
/// a future that transforms some input into an output
/// </summary>
/// <typeparam name="T">the input data type</typeparam>
/// <typeparam name="TOut">the output data type</typeparam>
public interface ITransformer<in T, TOut> : IFuture<TOut>
{
    TOut Next(T value);
    TOut Complete();
    void Error(Exception ex);
    void Cancel();

    ITransformer<T, TNext> Map<TNext>(Func<TOut, TNext> next);
    ITransformer<T, TNext> Map<TNext>(Func<TOut, Task<TNext>> next);
    ITransformer<T, TNext> Map<TNext>(ITransformer<TOut, TNext> next);

    public delegate void Resolver(T value, IConsumer<TOut> consumer);
}

/// <summary>
/// a future that transforms some input into an output
/// </summary>
/// <typeparam name="T1">the first input data type</typeparam>
/// <typeparam name="T2">the second input data type</typeparam>
/// <typeparam name="TOut">the output data type</typeparam>
public interface ITransformer<in T1, in T2, TOut> : IFuture<TOut>
{
    TOut Next(T1 a, T2 b);
    TOut Complete();
    void Error(Exception ex);
    void Cancel();

    ITransformer<T1, T2, TNext> Map<TNext>(Func<TOut, TNext> next);
    ITransformer<T1, T2, TNext> Map<TNext>(Func<TOut, Task<TNext>> next);
    ITransformer<T1, T2, TNext> Map<TNext>(ITransformer<TOut, TNext> next);

    public delegate void Resolver(T1 a, T2 b, IConsumer<TOut> consumer);
}