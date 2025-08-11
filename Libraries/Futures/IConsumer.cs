namespace Futures;

/// <summary>
/// consumes/reads data from some Producer
/// </summary>
/// <typeparam name="T">the type of data consumed</typeparam>
public interface IConsumer<in T>
{
    void OnNext(T value);
    void OnComplete();
    void OnError(Exception ex);
    void OnCancel();
}

/// <summary>
/// consumes/reads data from some Producer
/// </summary>
/// <typeparam name="T1">the first type of data consumed</typeparam>
/// <typeparam name="T2">the second type of data consumed</typeparam>
public interface IConsumer<in T1, in T2>
{
    void OnNext(T1 a, T2 b);
    void OnComplete();
    void OnError(Exception ex);
    void OnCancel();
}