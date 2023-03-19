namespace Xiphos.Threading
{
    internal static class ThreadSafePool<TChannel>
    {
        public static T Take<T>()
            where T : new()
            => ThreadSafePool<T, TChannel>.Take();

        public static void Add<T>(T item)
            where T : new()
            => ThreadSafePool<T, TChannel>.Add(item);
    }

    internal static class ThreadSafePool<T, TChannel>
        where T : new()
    {
        private static readonly ConcurrentBag<T> pool = new();

        public static T Take()
        {
            if (pool.TryTake(out T? result))
                return result;
            return new();
        }

        public static void Add(T item)
            => pool.Add(item);
    }

    internal static class ConstructedThreadSafePool<T, TChannel>
        where T : IConstructor<T>, new()
    {
        private static readonly ConcurrentBag<T> pool = new();

        public static T Take()
        {
            if (pool.TryTake(out T? result))
                return result;
            return new T().Constructor();
        }

        public static void Add(T item)
            => pool.Add(item);
    }

    internal interface IConstructor<T>
    {
        public Func<T> Constructor { get; }
    }
}
