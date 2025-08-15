namespace Futures;

public partial class Stream<T>
{
    public State State { get; protected set; } = State.NotStarted;

    public bool IsComplete => IsSuccess || IsError || IsCancelled;
    public bool IsStarted => State == State.Started;
    public bool IsSuccess => State == State.Success;
    public bool IsError => State == State.Error;
    public bool IsCancelled => State == State.Cancelled;

    protected Exception? Err { get; set; }
    protected List<Subscriber<T>> Consumers { get; } = [];

    public Stream(CancellationToken cancellation = default)
    {
        cancellation.Register(() =>
        {
            Cancel();
            UnSubscribe();
        });
    }

    public void Emit(T value)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("stream is already complete");
        }

        State = State.Started;

        foreach (var subscriber in Consumers)
        {
            subscriber.OnNext(value);
        }
    }

    public void Complete()
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("stream is already complete");
        }

        State = State.Success;

        foreach (var subscriber in Consumers)
        {
            subscriber.OnComplete();
        }
    }

    public void Error(Exception error)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("future is already complete");
        }

        State = State.Error;
        Err = error;

        foreach (var subscriber in Consumers)
        {
            subscriber.OnError(error);
        }
    }

    public void Cancel()
    {
        if (IsComplete) return;

        State = State.Cancelled;

        foreach (var subscriber in Consumers)
        {
            subscriber.OnCancel();
        }
    }

    protected void UnSubscribe()
    {
        foreach (var subscriber in Consumers)
        {
            subscriber.UnSubscribe();
        }
    }
}