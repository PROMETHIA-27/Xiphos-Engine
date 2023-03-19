namespace Xiphos.Monads;

public enum Maybe { None, Some }

public readonly struct Maybe<T> : IEquatable<Maybe<T>>, IEquatable<Maybe>
{
    private readonly T _value;
    private readonly Maybe _state;

    internal T Some => this == Maybe.Some ? this._value : throw new InvalidOperationException("Maybe is not in some state! Cannot extract value.");

    public Maybe(T value)
        => (this._value, this._state) = (value, Maybe.Some);

    public void Deconstruct(out Maybe state, out T value)
        => (state, value) = (this._state, this._value);

    public bool Equals(Maybe<T> other)
        => this._value is not null && this._value.Equals(other._value) && this._state == other._state;

    public bool Equals(Maybe other)
        => this._state == other;

    public override bool Equals(object? obj)
        => obj is Maybe<T> other && this.Equals(other);

    public static bool operator ==(Maybe<T> maybe, Maybe state)
        => maybe.Equals(state);

    public static bool operator !=(Maybe<T> maybe, Maybe state)
        => !maybe.Equals(state);

    public override int GetHashCode()
        => HashCode.Combine(this._value, this._state);

    public static Maybe<T> Flatten(Maybe<Maybe<T>> maybe)
        => maybe switch
        {
            (Maybe.Some, var inner) => inner,
            _ => new(),
        };

    public static Maybe<U> Map<U>(Func<T, U> map, Maybe<T> maybe)
        => maybe switch
        {
            (Maybe.Some, var value) => new(map(value)),
            _ => new(),
        };

    public void Match(Action<T> some, Action none)
    {
        switch (this)
        {
            case (Maybe.Some, var value):
                some(value);
                break;
            default:
                none();
                break;
        }
    }

    public Maybe<U> Match<U>(Func<T, U> some, Func<Maybe<U>> none)
        => this switch
        {
            (Maybe.Some, var value) => new(some(value)),
            _ => none(),
        };

    public Maybe<U> Select<U>(Func<T, U> select)
        => this switch
        {
            (Maybe.Some, var value) => new(select(value)),
            (Maybe.None, _) => new(),
            _ => throw new InvalidOperationException("Attempted to select invalid maybe!"),
        };

    public Maybe<T> Where(Func<T, bool> where)
        => this switch
        {
            (Maybe.Some, var value) when where(value) => this,
            (Maybe.Some or Maybe.None, _) => new(),
            _ => throw new InvalidOperationException("Attempted to use where on invalid maybe!"),
        };

    public static implicit operator bool(Maybe<T> maybe) => maybe._state == Maybe.Some;
}

public static class MaybeExtensions
{
    public static Maybe<T> Flatten<T>(this Maybe<Maybe<T>> maybe)
        => Maybe<T>.Flatten(maybe);
}
