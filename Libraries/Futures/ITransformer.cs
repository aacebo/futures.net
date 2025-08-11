namespace Futures;

/// <summary>
/// a <c>Future</c> that can write some <c>T</c>
/// input and returns some <c>TOut</c> output
/// </summary>
/// <typeparam name="T">the input type</typeparam>
/// <typeparam name="TOut">the output type</typeparam>
public interface ITransformer<T, TOut> : IFuture<TOut>, IConsumer<T>, IProducer<TOut>
{
    TOut Next(T value);
    Task<TOut> NextAsync(T value);

    TOut Complete();
    Task<TOut> CompleteAsync();

    void Error(Exception ex);
    Task ErrorAsync(Exception ex);

    void Cancel();
    Task CancelAsync();

    ITransformer<T, TNext> Map<TNext>(Func<TOut, TNext> next);
    ITransformer<T, TNext> Map<TNext>(Func<TOut, Task<TNext>> next);
    ITransformer<T, TOut> Map(IFuture<TOut> topic);
    ITransformer<T, TNext> Map<TNext>(ITransformer<TOut, TNext> transform);
}

/// <summary>
/// a <c>Future</c> that can write some <c>T1</c> and
/// <c>T2</c> input and returns some <c>TOut</c> output
/// </summary>
/// <typeparam name="T1">the first input type</typeparam>
/// <typeparam name="T2">the second input type</typeparam>
/// <typeparam name="TOut">the output type</typeparam>
public interface ITransformer<T1, T2, TOut> : IFuture<TOut>
{
    TOut Next(T1 a, T2 b);
    Task<TOut> NextAsync(T1 a, T2 b);

    TOut Complete();
    Task<TOut> CompleteAsync();

    void Error(Exception ex);
    Task ErrorAsync(Exception ex);

    void Cancel();
    Task CancelAsync();

    ITransformer<T1, T2, TNext> Map<TNext>(Func<TOut, TNext> next);
    ITransformer<T1, T2, TNext> Map<TNext>(Func<TOut, Task<TNext>> next);
    ITransformer<T1, T2, TOut> Map(IFuture<TOut> topic);
    ITransformer<T1, T2, TNext> Map<TNext>(ITransformer<TOut, TNext> transform);
}