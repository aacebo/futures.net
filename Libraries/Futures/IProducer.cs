namespace Futures;

/// <summary>
/// produces/writes data to some Consumer
/// </summary>
/// <typeparam name="T">the type of data produced</typeparam>
public interface IProducer<in T>
{
    void Next(T value);
    void Complete();
    void Error(Exception ex);
    void Cancel();
}

/// <summary>
/// produces/writes data to some Consumer
/// </summary>
/// <typeparam name="T1">the first type of data produced</typeparam>
/// <typeparam name="T2">the second type of data produced</typeparam>
public interface IProducer<in T1, in T2>
{
    void Next(T1 a, T2 b);
    void Complete();
    void Error(Exception ex);
    void Cancel();
}