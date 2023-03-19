namespace Xiphos.Monads;
#pragma warning disable IDE0032 // Use auto property
public enum Either : byte
{
    Left = 1,
    Right = 2,
}

public readonly unsafe struct Either<T, U> : IEquatable<Either>
    where T : unmanaged
    where U : unmanaged
{
    static Either()
    {
        if (sizeof(U) < sizeof(T))
            throw new ArgumentException("The byte size of U must be >= the byte size of T in an Either<T, U>");
    }

    private readonly Either _state;
    private readonly Union<T, U> _union;

    internal Either State => this._state;
    internal T Left => this == Either.Left ? this._union : default!;
    internal U Right => this == Either.Right ? this._union : default!;

    public T LeftProjection => this == Either.Left ? this.Left : throw new InvalidOperationException("Either not in left state!");
    public U RightProjection => this == Either.Right ? this.Right : throw new InvalidOperationException("Either not in right state!");

    public Maybe<T> LeftMaybe => this == Either.Left ? new(this.Left) : new();
    public Maybe<U> RightMaybe => this == Either.Right ? new(this.Right) : new();

    public Either(T value)
        => (this._union, this._state) = (value, Either.Left);

    public Either(U value)
        => (this._union, this._state) = (value, Either.Right);

    public void Deconstruct(out Either state, out T left, out U right)
        => (state, left, right) = (this._state, this.Left, this.Right);

    public void Deconstruct(out Either state, out U right)
        => (state, right) = (this._state, this.Right);

    public bool Equals(Either other)
        => this._state == other;

    public static bool operator ==(Either<T, U> either, Either state)
        => either.Equals(state);

    public static bool operator !=(Either<T, U> either, Either state)
        => !either.Equals(state);

    public override bool Equals(object? obj)
        => obj is Either<T, U> other && this.Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(this._union, this._state);

    public static Either<T, V> Flatten<V>(Either<T, Either<U, V>> either)
        where V : unmanaged
        => either switch
        {
            (Either.Left, var left, _) => new(left),
            (Either.Right, var right) => right switch
            {
                (Either.Left, _, _) => throw new InvalidOperationException("Flattened either was did not have a right value"),
                (Either.Right, var value) => new(value),
                _ => throw new InvalidOperationException("Attempted to flatten invalid either!"),
            },
            _ => throw new InvalidOperationException("Attempted to flatten invalid either!"),
        };

    public Either<T, U1> Map<U1>(Func<U, U1> map)
        where U1 : unmanaged
        => this.State == Either.Right ? new(map(this.Right)) : new(this.Left);

    public Either<T, U1> Map<U1>(delegate*<U, U1> map)
        where U1 : unmanaged
        => this.State == Either.Right ? new(map(this.Right)) : new(this.Left);

    public void Match(Action<T> left, Action<U> right)
    {
        switch (this)
        {
            case (Either.Left, var lval, _):
                left(lval);
                break;
            case (Either.Right, _, var rval):
                right(rval);
                break;
            default:
                throw new InvalidOperationException();
        }
    }

    public V Match<V>(Func<T, V> left, Func<U, V> right)
        => this switch
        {
            (Either.Left, var lval, _) => left(lval),
            (Either.Right, _, var rval) => right(rval),
            _ => throw new InvalidOperationException("Attempted to match invalid either!"),
        };

    public Either<V, W> Match<V, W>(Func<T, V> left, Func<U, W> right)
        where V : unmanaged
        where W : unmanaged
        => this switch
        {
            (Either.Left, var lval, _) => new(left(lval)),
            (Either.Right, _, var rval) => new(right(rval)),
            _ => throw new InvalidOperationException("Attempted to match invalid either!"),
        };

    public Either<T, V> Select<V>(Func<U, V> select)
        where V : unmanaged
        => this switch
        {
            (Either.Left, var lval, _) => new(lval),
            (Either.Right, _, var rval) => new(select(rval)),
            _ => throw new InvalidOperationException("Attempted to select on invalid either!"),
        };
}

public static class EitherExtensions
{
    public static Either<T, V> Flatten<T, U, V>(this Either<T, Either<U, V>> either)
        where T : unmanaged
        where U : unmanaged
        where V : unmanaged
        => Either<T, U>.Flatten(either);
}
