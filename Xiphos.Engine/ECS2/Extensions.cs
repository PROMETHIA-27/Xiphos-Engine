//namespace Xiphos.ECS2
//{
//    public partial class Extensions
//    {
//        public static T GetPart<T, U>(this U entity)
//            where U : IHasPart<T>
//        {
//            entity.GetPartRef(out PartRef<T> partRef);
//            return partRef.Value;
//        }

//        public static void GetPart<T, U>(this U entity, out T part)
//            where U : IHasPart<T>
//        {
//            entity.GetPartRef(out PartRef<T> partRef);
//            part = partRef.Value;
//        }

//        public static void SetPart<T, U>(ref this U entity, in T part)
//            where U : struct, IHasPart<T>
//        {
//            entity.GetPartRef(out PartRef<T> partRef);
//            partRef.Value = part;
//        }

//        public static void SetPart<T, U>(this U entity, in T part)
//            where U : class, IHasPart<T>
//        {
//            entity.GetPartRef(out PartRef<T> partRef);
//            partRef.Value = part;
//        }
//    }
//}
