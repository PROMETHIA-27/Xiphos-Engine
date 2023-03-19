//namespace Xiphos.Utilities;

//internal static class ComponentFunctionPointerHelper
//{
//    public static bool VerifyFunctionPointer<TSource>(MethodInfo m, Type returnType, ParameterAttributes[] paramAttributes, Type[] parameters, string errorName)
//        => m switch
//        {
//            { IsPublic: false } => throw new Exception($"{errorName} {typeof(TSource).Name}.{m.Name} must be public!"),
//            { IsStatic: false } => throw new Exception($"{errorName} {typeof(TSource).Name}.{m.Name} must be static!"),
//            { IsVirtual: true } => throw new Exception($"{errorName} {typeof(TSource).Name}.{m.Name} must not be virtual!"),
//            { ReturnType: var ret } when ret != returnType => throw new Exception($"{errorName} {typeof(TSource).Name}.{m.Name} must return void!"),
//            { IsGenericMethod: true } => throw new Exception($"{errorName} {typeof(TSource).Name}.{m.Name} must not be generic!"),
//            _ when m.GetParameters() switch
//            {
//                { Length: var l } when l != parameters.Length => throw new Exception($"{errorName} {typeof(TSource).Name}.{m.Name} must take exactly {parameters.Length} parameters!"),
//                var @params when
//                !(@params
//                .Select(p => p.ParameterType)
//                .SequenceEqual(parameters)
//                &&
//                @params
//                .Select(p => p.Attributes)
//                .SequenceEqual(paramAttributes))
//                    => throw new Exception($"{errorName} {typeof(TSource).Name}.{m.Name} must take the parameters ({ConstructParameterString(paramAttributes, parameters)})"),
//                _ => true
//            } => true,
//            _ => throw new Exception($"Unknown failure scanning {errorName} {typeof(TSource).Name}.{m.Name}!"),
//        };

//    static string ConstructParameterString(ParameterAttributes[] attribs, Type[] types)
//        => (from type in types
//            from attrib in attribs
//            select
//            (attrib.HasFlag(ParameterAttributes.Out) ? "out " : "") +
//            (attrib.HasFlag(ParameterAttributes.In) ? "in " : "") +
//            (type.IsByRef && attrib is not ParameterAttributes.Out or ParameterAttributes.In ? "ref " : "") +
//            type.Name)
//           .Aggregate((working, next) => working + " " + next);
//}