namespace Futures;

public partial class Stream<T> : IEquatable<Stream<T>>
{
    public bool Equals(Stream<T>? other)
    {
        return Id == other?.Id;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Stream<T>);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Stream<T> left, Stream<T> right) => left.Equals(right);
    public static bool operator !=(Stream<T> left, Stream<T> right) => !left.Equals(right);
}