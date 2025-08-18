namespace Futures;

public class Subscriber<T> : Subscription, IConsumer<T>, IDisposable, IEquatable<IIdentifiable>
{
    public Action<object, T>? Next { get; set; }
    public Action? Complete { get; set; }
    public Action<Exception>? Error { get; set; }
    public Action? Cancel { get; set; }

    internal Guid Id { get; set; }

    public Subscriber() : base()
    {
        Id = Guid.NewGuid();
    }

    public Subscriber(IConsumer<T> destination) : base()
    {
        Id = Guid.NewGuid();
        Next = destination.OnNext;
        Complete = destination.OnComplete;
        Error = destination.OnError;
        Cancel = destination.OnCancel;
    }

    public Subscriber(Future<T> future) : base()
    {
        Id = future.Id;
        Next = (sender, value) => future.Next(sender, value);
    }

    ~Subscriber()
    {
        Dispose();
    }

    public void OnNext(object sender, T value)
    {
        _count++;

        if (_limit != null && _count >= _limit)
        {
            UnSubscribe();
        }

        if (Next is not null)
        {
            Next(sender, value);
        }
    }

    public void OnComplete()
    {
        if (Complete is not null)
        {
            Complete();
        }
    }

    public void OnError(Exception error)
    {
        if (Error is not null)
        {
            Error(error);
        }
    }

    public void OnCancel()
    {
        if (Cancel is not null)
        {
            Cancel();
        }
    }

    public bool Equals(IIdentifiable? other)
    {
        return Id == other?.Id;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as IIdentifiable);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static Subscriber<T> From<TOut>(Future<T, TOut> future)
    {
        return new()
        {
            Id = future.Id,
            Next = (sender, value) => future.Next(sender, value),
        };
    }

    public static bool operator ==(Subscriber<T> left, IIdentifiable right) => left.Equals(right);
    public static bool operator !=(Subscriber<T> left, IIdentifiable right) => !left.Equals(right);
}