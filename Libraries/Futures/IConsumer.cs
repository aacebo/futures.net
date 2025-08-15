namespace Futures;

/// <summary>
/// consumes/reads data from some Producer
/// </summary>
/// <typeparam name="T">the type of data consumed</typeparam>
public interface IConsumer<T>
{
    void OnNext(T value);
    void OnComplete();
    void OnError(Exception ex);
    void OnCancel();
}