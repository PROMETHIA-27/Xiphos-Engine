//namespace Xiphos.ECS
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

//    public static partial class Extensions
//    {
//        public static ulong GetTypeIndex<T>(this T @object)
//            => TypeIndex<T>.Index;

//        public static T GetTypeIndex<T>(this T @object, out ulong typeIndex)
//        {
//            typeIndex = TypeIndex<T>.Index;
//            return @object;
//        }
//    }
//}
