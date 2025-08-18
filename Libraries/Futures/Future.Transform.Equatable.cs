namespace Futures;

public partial class Future<T, TOut> : IEquatable<IIdentifiable>
{
    public bool Equals(IIdentifiable? other)
    {
        return Id == other?.Id;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as IStreamable<TOut>);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Future<T, TOut> left, IIdentifiable right) => left.Equals(right);
    public static bool operator !=(Future<T, TOut> left, IIdentifiable right) => !left.Equals(right);
}

public partial class Future<T1, T2, TOut> : IEquatable<IIdentifiable>
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

    public static bool operator ==(Future<T1, T2, TOut> left, IIdentifiable right) => left.Equals(right);
    public static bool operator !=(Future<T1, T2, TOut> left, IIdentifiable right) => !left.Equals(right);
}