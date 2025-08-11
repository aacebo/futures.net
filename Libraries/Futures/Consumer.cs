namespace Futures;

/// <summary>
/// consumes/reads data from some Future
/// </summary>
/// <typeparam name="T">the type of data consumed</typeparam>
public class Consumer<T> : IConsumer<T>
{
    public Action<T>? Next { get; set; }
    public Func<T, Task>? NextAsync { get; set; }

    public Action? Complete { get; set; }
    public Func<Task>? CompleteAsync { get; set; }

    public Action<Exception>? Error { get; set; }
    public Func<Exception, Task>? ErrorAsync { get; set; }

    public Action? Cancel { get; set; }
    public Func<Task>? CancelAsync { get; set; }

    ~Consumer()
    {
        Dispose();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public void OnNext(T value)
    {
        if (Next is not null)
        {
            Next(value);
        }

        if (NextAsync is not null)
        {
            NextAsync(value);
        }
    }

    public void OnComplete()
    {
        if (Complete is not null)
        {
            Complete();
        }

        if (CompleteAsync is not null)
        {
            CompleteAsync();
        }
    }

    public void OnError(Exception error)
    {
        if (Error is not null)
        {
            Error(error);
        }

        if (ErrorAsync is not null)
        {
            ErrorAsync(error);
        }
    }

    public void OnCancel()
    {
        if (Cancel is not null)
        {
            Cancel();
        }

        if (CancelAsync is not null)
        {
            CancelAsync();
        }
    }
}