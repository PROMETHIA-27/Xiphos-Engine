namespace Xiphos.ECS2
{
    internal interface IEntityQuery { }

    public struct EntityQuery : IEntityQuery
    {
        public static bool Has<T>(in EntityStateCache cache)
            => new Has<T>().Evaluate(cache);

        public static bool Or<T, U>(in EntityStateCache cache)
            where T : IHasExpression, new()
            where U : IHasExpression, new()
            => new Or<T, U>().Evaluate(cache);

        public static bool And<T, U>(in EntityStateCache cache)
            where T : IHasExpression, new()
            where U : IHasExpression, new()
            => new And<T, U>().Evaluate(cache);

        public static bool Xor<T, U>(in EntityStateCache cache)
            where T : IHasExpression, new()
            where U : IHasExpression, new()
            => new Xor<T, U>().Evaluate(cache);
    }

    public struct EntityQuery<T> : IEntityQuery
        where T : IHasExpression, new()
    {
        public bool Evaluate(in EntityStateCache cache)
            => new T().Evaluate(cache);
    }

    public interface IHasExpression
    {
        public bool Evaluate(in EntityStateCache cache);
    }

    public struct Has<T> : IHasExpression
    {
        public bool Evaluate(in EntityStateCache cache)
            => cache.signature.GetOrFalse(TypeIndex<T>.Index) != 0;
    }

    public struct Has<T, U> : IHasExpression
    {
        public bool Evaluate(in EntityStateCache cache)
            => cache.signature.GetOrFalse(TypeIndex<T>.Index) != 0 &&
            cache.signature.GetOrFalse(TypeIndex<U>.Index) != 0;
    }

    public struct Has<T, U, V> : IHasExpression
    {
        public bool Evaluate(in EntityStateCache cache)
            => cache.signature.GetOrFalse(TypeIndex<T>.Index) != 0 &&
            cache.signature.GetOrFalse(TypeIndex<U>.Index) != 0 &&
            cache.signature.GetOrFalse(TypeIndex<V>.Index) != 0;
    }

    public struct HasNone<T> : IHasExpression
    {
        public bool Evaluate(in EntityStateCache cache)
            => cache.signature.GetOrFalse(TypeIndex<T>.Index) == 0;
    }

    public struct HasNone<T, U> : IHasExpression
    {
        public bool Evaluate(in EntityStateCache cache)
            => cache.signature.GetOrFalse(TypeIndex<T>.Index) == 0 &&
            cache.signature.GetOrFalse(TypeIndex<U>.Index) == 0;
    }

    public struct HasNone<T, U, V> : IHasExpression
    {
        public bool Evaluate(in EntityStateCache cache)
            => cache.signature.GetOrFalse(TypeIndex<T>.Index) == 0 &&
            cache.signature.GetOrFalse(TypeIndex<U>.Index) == 0 &&
            cache.signature.GetOrFalse(TypeIndex<V>.Index) == 0;
    }

    public struct Or<T, U> : IHasExpression
        where T : IHasExpression, new()
        where U : IHasExpression, new()
    {
        public bool Evaluate(in EntityStateCache cache)
            => new T().Evaluate(cache) || new U().Evaluate(cache);
    }

    public struct And<T, U> : IHasExpression
        where T : IHasExpression, new()
        where U : IHasExpression, new()
    {
        public bool Evaluate(in EntityStateCache cache)
            => new T().Evaluate(cache) && new U().Evaluate(cache);
    }

    public struct Xor<T, U> : IHasExpression
        where T : IHasExpression, new()
        where U : IHasExpression, new()
    {
        public bool Evaluate(in EntityStateCache cache)
            => new T().Evaluate(cache) ^ new U().Evaluate(cache);
    }
}
