namespace Futures;

/// <summary>
/// consumes/reads data from some Producer
/// </summary>
/// <typeparam name="T">the type of data consumed</typeparam>
public interface IConsumer<T>
{
    void Next(T value);
    void Complete();
    void Error(Exception ex);
    void Cancel();
}

/// <summary>
/// consumes/reads data from some Producer
/// </summary>
/// <typeparam name="T">the type of data consumed</typeparam>
/// <typeparam name="TOut">the type of data output</typeparam>
public interface IConsumer<T, TOut>
{
    void Next(T value, Action<TOut>? result = null);
    void Complete(Action<TOut>? result = null);
    void Error(Exception ex);
    void Cancel();
}

/// <summary>
/// consumes/reads data from some Producer
/// </summary>
/// <typeparam name="T1">the first type of data consumed</typeparam>
/// <typeparam name="T2">the second type of data consumed</typeparam>
/// <typeparam name="TOut">the type of data output</typeparam>
public interface IConsumer<T1, T2, TOut>
{
    TOut Next(T1 a, T2 b, Action<TOut>? result = null);
    TOut Complete(Action<TOut>? result = null);
    void Error(Exception ex);
    void Cancel();
}
