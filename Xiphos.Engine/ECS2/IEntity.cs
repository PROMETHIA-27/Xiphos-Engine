//using XiphosCodeGeneration;

//namespace Xiphos.ECS2
//{
//    public interface IEntity 
//    {
//        protected static PartRef<T> GetPartRef<T>(ref T item)
//            => new(ref item);
//    }

//    /// <summary>
//    /// An interface to provide methods for manipulating a part of an entity
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    public interface IHasPart<T> : IEntity
//    {
//        public void GetPartRef(out PartRef<T> part);
//    }

//    [InitialEntities(4)]
//    [AutoImplPart]
//    [SubscribeToEvent(typeof(FrameStartEvent))]
//    public partial struct Player : IHasPart<Transform>, IHasPart<Physics>
//    {
//        public Transform trans;
//        public Physics phys;
//    }

//    public struct Transform
//    {
//        public int x;
//        int y;
//        int z;
//    }

//    public struct Physics
//    {
//        public int speed;
//        float direction;
//        float mass;
//        float drag;
//    }

//    public struct EntityIndex
//    {
//        internal int index;
//        internal int version;
//    }

//    public readonly ref struct PartRef<T>
//    {
//        internal Ref<T> Ref { get; init; }

//        internal ref T Value => ref this.Ref.Value;

//        internal PartRef(ref T part)
//            => this.Ref = new(ref part);
//    }

//    /// <summary>
//    /// An unsafe reference to something
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    internal unsafe ref struct Ref<T>
//    {
//        readonly void* _data;

//        /// <summary>
//        /// The referenced value
//        /// </summary>
//        public ref T Value => ref Unsafe.AsRef<T>(this._data);

//        public Ref(ref T target)
//            => this._data = Unsafe.AsPointer(ref target);
//    }
//}