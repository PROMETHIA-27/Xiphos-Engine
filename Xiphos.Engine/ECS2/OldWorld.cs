namespace Xiphos.ECS2
{
    public struct OldWorld
    {
        //public const int DEFAULT_INITIAL_ENTITIES = 1024;

        //public OldWorld(int initialCapacity)
        //    => (this.entityCaches, this.eventBuses) = (new(initialCapacity), new());

        //internal readonly Dictionary<int, IEntityCache> entityCaches;
        //internal readonly Dictionary<int, IEventBus> eventBuses;
        //internal static readonly Dictionary<int, int[]> eventSubscriptions = new();

        //static int GetInitialEntities<T>()
        //{
        //    var type = typeof(T);
        //    return ((int?)type.CustomAttributes
        //        .Where(a => a.AttributeType == typeof(InitialEntitiesAttribute))
        //        .FirstOrDefault()
        //        ?.ConstructorArguments
        //        ?.FirstOrDefault()
        //        .Value) ?? DEFAULT_INITIAL_ENTITIES;
        //}

        //public bool TryGetEntityCache<T>([NotNullWhen(true)] out EntityCache<T>? cache)
        //    where T : IEntity
        //{
        //    if (this.entityCaches.TryGetValue(TypeIndex<T>.Index, out var uncastCache))
        //    {
        //        cache = (EntityCache<T>)uncastCache;
        //        return true;
        //    }
        //    cache = default;
        //    return false;
        //}

        //public EntityCache<T> GetOrCreateEntityCache<T>()
        //    where T : IEntity
        //{
        //    if (!this.entityCaches.TryGetValue(TypeIndex<T>.Index, out var cache))
        //    {
        //        cache = new EntityCache<T>(GetInitialEntities<T>());
        //        this.entityCaches.Add(TypeIndex<T>.Index, cache!);

        //        if (!eventSubscriptions.ContainsKey(TypeIndex<T>.Index))
        //            eventSubscriptions.Add(TypeIndex<T>.Index, GetTypeEventSubscriptions<T>());
        //    }
        //    return (EntityCache<T>)cache;
        //}

        //static int[] GetTypeEventSubscriptions<T>()
        //{
        //    var type = typeof(T);
        //    return type
        //        .CustomAttributes
        //        .Where(a => a.AttributeType == typeof(SubscribeToEventAttribute))
        //        .FirstOrDefault()
        //        ?.ConstructorArguments
        //        ?.SelectMany(args => (ReadOnlyCollection<CustomAttributeTypedArgument>)(args.Value!))
        //        ?.Select(arg => ((Type)arg.Value!).
        //        ())
        //        ?.ToArray() ?? new int[0];
        //}
    }
}
