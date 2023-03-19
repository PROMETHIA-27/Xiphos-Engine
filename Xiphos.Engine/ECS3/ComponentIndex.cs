namespace Xiphos.ECS3;

internal static class ComponentIndex
{
    internal static ulong nextIdx = 0;
}

internal static class ComponentIndex<T>
    where T : IComponent
{
    static ComponentIndex()
    {
        Index = ComponentIndex.nextIdx;
        ComponentIndex.nextIdx++;
    }

    public static ulong Index { get; private set; }
}

internal static partial class Extensions
{
    public static ulong ComponentIndex<T>(this T @object)
        where T : IComponent
        => ECS3.ComponentIndex<T>.Index;

    public static T ComponentIndex<T>(this T @object, out ulong typeIndex)
        where T : IComponent
    {
        typeIndex = ECS3.ComponentIndex<T>.Index;
        return @object;
    }

    public static ulong GetComponentIndex(this Type type)
    {
        if (!type.IsAssignableTo(typeof(IComponent)))
            throw new ArgumentException($"Type {type.FullName} does not implement {nameof(IComponent)}, and cannot be indexed as a component.");

        MethodInfo? IndexAccessor = typeof(ComponentIndex<>)
            .MakeGenericType(type)
            ?.GetProperty("Index", BindingFlags.Static | BindingFlags.Public)
            ?.GetGetMethod() ?? throw new Exception($"ComponentIndex for type {type.Name} does not exist. That shouldn't be happening.");

        return (ulong)(IndexAccessor.Invoke(null, null) ?? throw new Exception($"Failed to invoke index accessor for component index of {type.Name}. That shouldn't be happening."));
    }
}
