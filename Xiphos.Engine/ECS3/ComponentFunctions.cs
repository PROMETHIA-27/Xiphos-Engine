namespace Xiphos.ECS3;

internal static unsafe class ComponentFunctions<T>
    where T : unmanaged, IComponent
{
    public static Dictionary<EntityState.ComponentFunctions, IntPtr> Functions { get; } = new();

    static ComponentFunctions()
    {
        Functions[EntityState.ComponentFunctions.Initialize] =
            typeof(T).GetInterface(typeof(IInitializer<>).Name) is not null
            ? typeof(T).GetMethod("Initialize")!.MethodHandle.GetFunctionPointer()
            : IntPtr.Zero;

        Functions[EntityState.ComponentFunctions.Cleanup] =
            typeof(T).GetInterface(typeof(ICleanupResponder<>).Name) is not null
            ? typeof(T).GetMethod("Cleanup")!.MethodHandle.GetFunctionPointer()
            : IntPtr.Zero;

        Functions[EntityState.ComponentFunctions.Filter] =
            typeof(T).GetInterfaces()
            .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IFilter<,>))
            .ToArray()
            switch
            {
                { Length: > 1 } => throw new Exception($"Type {typeof(T).Name} cannot implement {typeof(IFilter<,>).Name} multiple times!"),
                { Length: < 1 } => IntPtr.Zero,
                var inter when inter[0].GenericTypeArguments[0] is Type fvType
                => typeof(T).GetMethod("CheckComponent")!.MethodHandle.GetFunctionPointer(),
                _ => throw new Exception($"Unknown failure scanning {typeof(IFilter<,>).Name} of {typeof(T).Name}"),
            };
    }
}

public interface IComponentFunction<TSelf>
    where TSelf : IComponentFunction<TSelf> { }

public interface ICleanupResponder<TSelf> : IComponentFunction<TSelf>
    where TSelf : ICleanupResponder<TSelf>
{
    static abstract void Cleanup(ref TSelf @this);
}

public interface IInitializer<TSelf> : IComponentFunction<TSelf>
    where TSelf : IInitializer<TSelf>
{
    static abstract void Initialize(out TSelf @this);
}

public interface IFilter<TValue, TSelf> : IComponentFunction<TSelf>
    where TSelf : IFilter<TValue, TSelf>
    where TValue : unmanaged
{
    static abstract bool CheckQuery(in TValue stateValue, in TValue comparisonValue);

    static abstract bool CheckComponent(in TValue stateValue, in TSelf self);
}