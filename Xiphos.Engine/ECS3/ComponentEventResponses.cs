namespace Xiphos.ECS3;

internal static class ComponentEventResponses
{
    public static readonly Dictionary<ulong, Dictionary<ulong, IntPtr>> componentsToResponses = new();
}

internal static class ComponentEventResponses<T>
    where T : unmanaged, IComponent
{
    public static readonly Dictionary<ulong, IntPtr> responses = new();

    static ComponentEventResponses()
    {
        ComponentEventResponses.componentsToResponses.Add(ComponentIndex<T>.Index, responses);

        _ = (from inter in typeof(T).FindInterfaces(
                (type, _) =>
                type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(IEventResponder<,>), null)
             let eventType = inter.GenericTypeArguments[0]
             select responses.TryAdd(eventType.GetEventIndex(), typeof(T).GetMethod("Respond",
                new[]
                {
                    eventType.MakeByRefType(),
                    typeof(T).MakeByRefType(),
                })!.MethodHandle.GetFunctionPointer()))
             .ToArray();

        _ = (from inter in typeof(T).FindInterfaces(
                (type, _) =>
                type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(IMessageResponder<,>), null)
             let eventType = inter.GenericTypeArguments[0]
             select responses.TryAdd(
                eventType.GetMessageIndex(), typeof(T).GetMethod("Respond",
                new[]
                {
                    eventType.MakeByRefType(),
                    typeof(T).MakeByRefType(),
                })!.MethodHandle.GetFunctionPointer()))
             .ToArray();

        //var ptrs = typeof(T)
        //    .FindInterfaces(
        //        (type, _) => 
        //        type.IsGenericType 
        //        && type.GetGenericTypeDefinition() == typeof(IComponentEventResponse<,>), null)
        //    .Select(inter => typeof(T).GetMethod($"Respond",
        //                new[]
        //                {
        //                    inter.GenericTypeArguments[0].MakeByRefType(),
        //                    inter.GenericTypeArguments[1].MakeByRefType()
        //                })!.MethodHandle.GetFunctionPointer())
        //    .Select(fnptr =>
        //    {
        //        responses.Add()
        //    });

        //_ = (from m in typeof(T).GetMethods()
        //     where m.CustomAttributes.Any(a => a.AttributeType == typeof(EventResponseAttribute))
        //     let eventType = m.CustomAttributes
        //       .First(a => a.AttributeType == typeof(EventResponseAttribute))
        //       .ConstructorArguments
        //       .First()
        //       .Value is Type t && t.IsAssignableTo(typeof(IComponentEvent))
        //       ? t
        //       : throw new ArgumentException($"Event response {typeof(T).Name}.{m.Name} must implement {nameof(IComponentEvent)}!")
        //     where ComponentFunctionPointerHelper.VerifyFunctionPointer<T>(
        //         m,
        //         typeof(void),
        //         new[] { ParameterAttributes.In, ParameterAttributes.None },
        //         new[] { eventType.MakeByRefType(), typeof(T).MakeByRefType() },
        //         "Event response")
        //     select Add(eventType, m.MethodHandle.GetFunctionPointer()))
        //     .ToArray();

        //static bool Add(Type eventType, IntPtr ptr)
        //    => responses.TryAdd(eventType.GetEventIndex(), ptr)
        //        ? true
        //        : throw new InvalidOperationException($"Type {typeof(T).Name} cannot have two responses to the same event {eventType.Name}!");
    }
}

public interface IEventResponder<TEvent, TSelf>
    where TEvent : unmanaged, IComponentEvent
    where TSelf : IEventResponder<TEvent, TSelf>
{
    static abstract void Respond(in TEvent e, ref TSelf @this);
}

public interface IMessageResponder<TMessage, TSelf>
    where TMessage : unmanaged, IComponentMessage
    where TSelf : IMessageResponder<TMessage, TSelf>
{
    static abstract void Respond(in TMessage m, ref TSelf @this);
}

public interface IComponentEvent { }

public interface IComponentMessage { }