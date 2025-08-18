namespace Futures;

public partial class Stream<T> : IEquatable<IIdentifiable>
{
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

    public static bool operator ==(Stream<T> left, IIdentifiable right) => left.Equals(right);
    public static bool operator !=(Stream<T> left, IIdentifiable right) => !left.Equals(right);
}