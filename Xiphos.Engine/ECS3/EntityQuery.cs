namespace Xiphos.ECS3;

public static class EntityQuery
{
    public static Has<T> Has<T>()
        where T : unmanaged, IComponent
        => new();

    public static Or<T, U> Or<T, U>(T lhs, U rhs)
        where T : IQueryExpression
        where U : IQueryExpression
        => new(lhs, rhs);

    public static And<T, U> And<T, U>(T lhs, U rhs)
        where T : IQueryExpression
        where U : IQueryExpression
        => new(lhs, rhs);

    public static Not<T> Not<T>(T expr)
        where T : IQueryExpression
        => new(expr);

    public static Xor<T, U> Xor<T, U>(T lhs, U rhs)
        where T : IQueryExpression
        where U : IQueryExpression
        => new(lhs, rhs);

    public static RespondsTo<T> RespondsTo<T>()
        where T : unmanaged, IComponentEvent
        => new();

    public static MatchesFilter<T, U> MatchesFilter<T, U>(in U filterValue)
        where T : unmanaged, IComponent, IFilter<U, T>
        where U : unmanaged
        => new(filterValue);
}

public interface IQueryExpression
{
    public bool Evaluate(in EntityState state);
}

public readonly struct Has<T> : IQueryExpression
    where T : unmanaged, IComponent
{
    public bool Evaluate(in EntityState state)
        => state.HasComponent<T>();
}

public readonly struct Has<T, U> : IQueryExpression
    where T : unmanaged, IComponent
    where U : unmanaged, IComponent
{
    public bool Evaluate(in EntityState state)
        => state.HasComponent<T>() &&
        state.HasComponent<U>();
}

public readonly struct Has<T, U, V> : IQueryExpression
    where T : unmanaged, IComponent
    where U : unmanaged, IComponent
    where V : unmanaged, IComponent
{
    public bool Evaluate(in EntityState state)
        => state.HasComponent<T>() &&
        state.HasComponent<U>() &&
        state.HasComponent<V>();
}

public readonly struct HasNone<T> : IQueryExpression
    where T : unmanaged, IComponent
{
    public bool Evaluate(in EntityState state)
        => !state.HasComponent<T>();
}

public readonly struct HasNone<T, U> : IQueryExpression
    where T : unmanaged, IComponent
    where U : unmanaged, IComponent
{
    public bool Evaluate(in EntityState state)
        => !state.HasComponent<T>() &&
        !state.HasComponent<U>();
}

public readonly struct HasNone<T, U, V> : IQueryExpression
    where T : unmanaged, IComponent
    where U : unmanaged, IComponent
    where V : unmanaged, IComponent
{
    public bool Evaluate(in EntityState state)
        => !state.HasComponent<T>() &&
        !state.HasComponent<U>() &&
        !state.HasComponent<V>();
}

public readonly struct Or<T, U> : IQueryExpression
    where T : IQueryExpression
    where U : IQueryExpression
{
    private readonly T _lhs;
    private readonly U _rhs;

    public bool Evaluate(in EntityState state)
        => this._lhs.Evaluate(state) || this._rhs.Evaluate(state);

    public Or(T lhs, U rhs)
        => (this._lhs, this._rhs) = (lhs, rhs);
}

public readonly struct And<T, U> : IQueryExpression
    where T : IQueryExpression
    where U : IQueryExpression
{
    private readonly T _lhs;
    private readonly U _rhs;

    public bool Evaluate(in EntityState state)
        => this._lhs.Evaluate(state) && this._rhs.Evaluate(state);

    public And(T lhs, U rhs)
        => (this._lhs, this._rhs) = (lhs, rhs);
}

public readonly struct Not<T> : IQueryExpression
    where T : IQueryExpression
{
    private readonly T _expr;

    public bool Evaluate(in EntityState state)
        => !this._expr.Evaluate(state);

    public Not(T expr)
        => this._expr = expr;
}

public readonly struct Xor<T, U> : IQueryExpression
    where T : IQueryExpression
    where U : IQueryExpression
{
    private readonly T _lhs;
    private readonly U _rhs;

    public bool Evaluate(in EntityState state)
        => this._lhs.Evaluate(state) ^ this._rhs.Evaluate(state);

    public Xor(T lhs, U rhs)
        => (this._lhs, this._rhs) = (lhs, rhs);
}

public readonly struct RespondsTo<T> : IQueryExpression
    where T : unmanaged, IComponentEvent
{
    public bool Evaluate(in EntityState state)
        => state.HandlesEvent<T>();
}

public readonly struct MatchesFilter<T, U> : IQueryExpression
    where T : unmanaged, IComponent, IFilter<U, T>
    where U : unmanaged
{
    private readonly U _filterValue;

    public bool Evaluate(in EntityState state)
        => state.GetFilterValue<T, U>() is var stateValueMaybe
        && stateValueMaybe
        && T.CheckQuery(stateValueMaybe.Some, this._filterValue);

    public MatchesFilter(in U filterValue)
        => this._filterValue = filterValue;
}
