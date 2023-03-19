namespace Xiphos.Utilities;
#pragma warning disable IDE0032 // Use auto property
internal readonly unsafe struct Union<T, U>
    where T : unmanaged
    where U : unmanaged
{
    static Union()
    {
        if (sizeof(U) < sizeof(T))
            throw new ArgumentException("The byte size of U must be >= the byte size of T in a Union<T, U>");
    }

    private readonly U _value;

    internal T View1 => this._value.Cast<U, T>();
    internal U View2 => this._value;

    public Union(T value)
        => this._value = value.CreateFromBytes<T, U>();

    public Union(U value)
        => this._value = value;

    public static implicit operator T(Union<T, U> union) => union.View1;
    public static implicit operator U(Union<T, U> union) => union.View2;

    public static implicit operator Union<T, U>(T value) => new(value);
    public static implicit operator Union<T, U>(U value) => new(value);
}
