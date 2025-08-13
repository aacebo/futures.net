namespace Futures;

/// <summary>
/// consumes/reads data from some Producer
/// </summary>
/// <typeparam name="T">the type of data consumed</typeparam>
public interface IConsumer<in T>
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
public interface IConsumer<in T, out TOut>
{
    TOut Next(T value);
    TOut Complete();
    void Error(Exception ex);
    void Cancel();
}

/// <summary>
/// consumes/reads data from some Producer
/// </summary>
/// <typeparam name="T1">the first type of data consumed</typeparam>
/// <typeparam name="T2">the second type of data consumed</typeparam>
/// <typeparam name="TOut">the type of data output</typeparam>
public interface IConsumer<in T1, in T2, out TOut>
{
    TOut Next(T1 a, T2 b);
    TOut Complete();
    void Error(Exception ex);
    void Cancel();
}
