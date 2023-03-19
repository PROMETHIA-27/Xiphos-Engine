namespace Xiphos.ECS3;

internal static class EventIndex
{
    internal static ulong _nextIdx = 0;
}

internal static class EventIndex<T>
    where T : IComponentEvent
{
    static EventIndex()
    {
        Index = EventIndex._nextIdx;
        EventIndex._nextIdx++;
    }

    public static ulong Index { get; private set; }
}

internal static class MessageIndex<T>
    where T : IComponentMessage
{
    static MessageIndex()
    {
        Index = EventIndex._nextIdx;
        EventIndex._nextIdx++;
    }

    public static ulong Index { get; private set; }
}

internal static partial class Extensions
{
    public static ulong EventIndex<T>(this T @object)
        where T : IComponentEvent
        => ECS3.EventIndex<T>.Index;

    public static T EventIndex<T>(this T @object, out ulong typeIndex)
        where T : IComponentEvent
    {
        typeIndex = ECS3.EventIndex<T>.Index;
        return @object;
    }

    public static ulong GetEventIndex(this Type type)
    {
        if (!type.IsAssignableTo(typeof(IComponentEvent)))
            throw new ArgumentException($"Type {type.FullName} does not implement {nameof(IComponentEvent)}, and cannot be indexed as an event.");

        MethodInfo? indexAccessor = typeof(EventIndex<>)
            .MakeGenericType(type)
            ?.GetProperty("Index", BindingFlags.Static | BindingFlags.Public)
            ?.GetGetMethod() ?? throw new Exception($"EventIndex for type {type.Name} does not exist. That shouldn't be happening.");

        return (ulong)(indexAccessor.Invoke(null, null) ?? throw new Exception($"Failed to invoke index accessor for event index of {type.Name}. That shouldn't be happening."));
    }
    public static ulong MessageIndex<T>(this T @object)
        where T : IComponentMessage
        => ECS3.MessageIndex<T>.Index;

    public static T MessageIndex<T>(this T @object, out ulong typeIndex)
        where T : IComponentMessage
    {
        typeIndex = ECS3.MessageIndex<T>.Index;
        return @object;
    }

    public static ulong GetMessageIndex(this Type type)
    {
        if (!type.IsAssignableTo(typeof(IComponentMessage)))
            throw new ArgumentException($"Type {type.FullName} does not implement {nameof(IComponentMessage)}, and cannot be indexed as an event.");

        MethodInfo? indexAccessor = typeof(MessageIndex<>)
            .MakeGenericType(type)
            ?.GetProperty("Index", BindingFlags.Static | BindingFlags.Public)
            ?.GetGetMethod() ?? throw new Exception($"MessageIndex for type {type.Name} does not exist. That shouldn't be happening.");

        return (ulong)(indexAccessor.Invoke(null, null) ?? throw new Exception($"Failed to invoke index accessor for message index of {type.Name}. That shouldn't be happening."));
    }
}
