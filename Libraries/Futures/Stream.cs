namespace Futures;

/// <summary>
/// a stream of data that acts as
/// an emitter (send and forget)
/// </summary>
public partial class Stream<T> : IStreamable<T>
{
    public Guid Id { get; } = Guid.NewGuid();
    public State State { get; protected set; } = State.NotStarted;

    public bool IsComplete => IsSuccess || IsError || IsCancelled;
    public bool IsStarted => State == State.Started;
    public bool IsSuccess => State == State.Success;
    public bool IsError => State == State.Error;
    public bool IsCancelled => State == State.Cancelled;

    internal CancellationToken Token { get; }
    protected Exception? Err { get; set; }
    protected List<Subscriber<T>> Consumers { get; } = [];

    public Stream(CancellationToken cancellation = default)
    {
        Token = cancellation;
        cancellation.Register(() =>
        {
            Cancel();
            UnSubscribe();
        });
    }

    public Stream(Stream<T> stream)
    {
        State = stream.State;
        Err = stream.Err;
        Token = stream.Token;
        Consumers = stream.Consumers;
    }

    protected void Emit(object sender, T value)
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("stream is already complete");
        }

        State = State.Started;

        foreach (var subscriber in Consumers)
        {
            if (subscriber.Equals(sender)) continue;
            subscriber.OnNext(sender, value);
        }
    }

    public virtual void Success()
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

    public virtual void Error(Exception error)
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

    public virtual void Cancel()
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