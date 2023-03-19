namespace Xiphos.ECS3;

internal static class ComponentMessageResponses<T>
    where T : unmanaged, IComponent
{
    //static ComponentMessageResponses() =>
    //    _ = (from inter in typeof(T).FindInterfaces(
    //            (type, _) =>
    //            type.IsGenericType &&
    //            type.GetGenericTypeDefinition() == typeof(IComponentMessageResponse<,>), null)
    //         let eventType = inter.GenericTypeArguments[0]
    //         select ComponentEventResponses<T>.responses.TryAdd(
    //            eventType.GetEventIndex(), typeof(T).GetMethod("Respond",
    //            new[]
    //            {
    //                eventType.MakeByRefType(),
    //                typeof(T).MakeByRefType(),
    //            })!.MethodHandle.GetFunctionPointer()))
    //         .ToArray();
    //_ = (from m in typeof(T).GetMethods()
    //     where m.CustomAttributes.Any(a => a.AttributeType == typeof(MessageResponseAttribute))
    //     let messageType = m.CustomAttributes
    //       .First(a => a.AttributeType == typeof(MessageResponseAttribute))
    //       .ConstructorArguments
    //       .First()
    //       .Value is Type t && t.IsAssignableTo(typeof(IComponentMessage))
    //       ? t
    //       : throw new ArgumentException($"Message response {typeof(T).Name}.{m.Name} must implement IComponentMessage!")
    //     where ComponentFunctionPointerHelper.VerifyFunctionPointer<T>(
    //         m,
    //         typeof(void),
    //         new[] { ParameterAttributes.In, ParameterAttributes.None },
    //         new[] { messageType.MakeByRefType(), typeof(T).MakeByRefType() },
    //         "Message response")
    //     select Add(messageType.GetMessageIndex(), m.MethodHandle.GetFunctionPointer()))
    //     .ToArray();
    //static bool Add(ulong index, IntPtr ptr)
    //{
    //    ComponentEventResponses<T>.responses.Add(index, ptr);
    //    return true;
    //}

    public static void Init() { }
}