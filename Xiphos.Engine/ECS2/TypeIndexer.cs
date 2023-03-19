//namespace Xiphos.ECS2
//{
//    internal static class TypeIndex
//    {
//        internal static ulong nextIdx = 0;
//    }

//    internal static class TypeIndex<T>
//    {
//        private static ulong idx;

//        static TypeIndex()
//        {
//            TypeIndex<T>.idx = TypeIndex.nextIdx;
//            TypeIndex.nextIdx++;
//        }

//        public static ulong Index => TypeIndex<T>.idx;
//    }

//    internal static partial class Extensions
//    {
//        public static ulong TypeIndex<T>(this T @object)
//            => ECS2.TypeIndex<T>.Index;

//        public static T TypeIndex<T>(this T @object, out ulong typeIndex)
//        {
//            typeIndex = ECS2.TypeIndex<T>.Index;
//            return @object;
//        }

//        public static ulong GetTypeIndex(this Type type)
//        {
//            MethodInfo? IndexAccessor = typeof(TypeIndex<>)
//                .MakeGenericType(type)
//                ?.GetProperty("Index", BindingFlags.Static | BindingFlags.Public)
//                ?.GetGetMethod() ?? throw new Exception($"TypeIndexer for type {type.Name} does not exist. That shouldn't be happening.");

//            return (ulong)(IndexAccessor.Invoke(null, null) ?? throw new Exception($"Failed to invoke index accessor for type indexer of {type.Name}. This should not be happening."));
//        }
//    }
//}
