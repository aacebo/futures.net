namespace Futures;

/// <summary>
/// consumes/reads data from some Producer
/// </summary>
/// <typeparam name="T">the type of data consumed</typeparam>
/// <typeparam name="TOut">the type of data returned</typeparam>
public interface IConsumer<T, TOut>
{
    IEnumerable<TOut> OnNext(T value);
    void OnComplete();
    void OnError(Exception ex);
    void OnCancel();
}
