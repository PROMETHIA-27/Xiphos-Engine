//namespace Xiphos.ECS
//{
//    public class Archetype : IDisposable, IEquatable<Archetype>
//    {
//        internal World world;
//        internal Dictionary<ulong, IComponentArray> componentArrays;
//        internal List<int> versions;
//        internal readonly Queue<int> freelist;

//        public int Index { get; internal init; }
//        public int Count { get; internal set; }
//        public int LastIndex { get; internal set; }

//        internal Archetype(World world, int index)
//        {
//            this.world = world;
//            this.componentArrays = new();
//            this.versions = ThreadSafePool<List<int>, XiphosStatic>.Take();
//            this.freelist = ThreadSafePool<Queue<int>, XiphosStatic>.Take();
//            this.Index = index;
//            this.Count = 0;
//            this.LastIndex = 0;
//        }

//        public void Dispose()
//        {
//            this.versions.Clear();
//            ThreadSafePool<XiphosStatic>.Add(this.versions);
//            this.freelist.Clear();
//            ThreadSafePool<XiphosStatic>.Add(this.freelist);
//            foreach (IComponentArray? arr in this.componentArrays.Values)
//                arr.Dispose();
//        }

//        internal (int idx, int version) AddEntity()
//        {
//            int idx;
//            int lastVersion;
//            if (this.freelist.Count > 0)
//            {
//                idx = this.freelist.Dequeue();
//                foreach (IComponentArray? array in this.componentArrays.Values)
//                    array.ResetComponent(idx);
//                lastVersion = this.versions[idx];
//            }
//            else
//            {
//                foreach (IComponentArray? array in this.componentArrays.Values)
//                    array.AddComponent();
//                idx = this.LastIndex;
//                this.LastIndex++;
//                lastVersion = -1;
//            }

//            return (idx, lastVersion);
//        }

//        public Entity CreateEntity()
//        {
//            (int idx, int lastVersion) = this.AddEntity();

//            this.Count++;
//            this.versions[idx]++;

//            return new Entity
//            {
//                WorldIdx = this.world.Index,
//                ArchetypeIdx = this.Index,
//                EntityIdx = idx,
//                Version = lastVersion
//            };
//        }

//        public Entity[] CreateEntities(int amount)
//        {
//            Entity[] entities = new Entity[amount];

//            for (int i = 0; i < amount; i++)
//            {
//                entities[i] = this.CreateEntity();
//            }

//            return entities;
//        }

//        public void DeleteEntity(Entity entity)
//        {
//            this.ThrowIfEntityNotMatch(entity);

//            this.freelist.Enqueue(entity.EntityIdx);
//            this.Count--;
//        }

//        public Entity MoveEntity(Entity entity, in Archetype archetype, bool copy = false)
//        {
//            this.ThrowIfEntityNotMatch(entity);

//            (int idx, int version) = archetype.AddEntity();

//            foreach (KeyValuePair<ulong, IComponentArray> arrayPair in this.componentArrays)
//            {
//                if (archetype.componentArrays.ContainsKey(arrayPair.Key))
//                    arrayPair.Value.CopyComponent(entity.EntityIdx, archetype.componentArrays[arrayPair.Key], idx);
//            }

//            if (!copy)
//                this.DeleteEntity(entity);

//            return new()
//            {
//                WorldIdx = archetype.world.Index,
//                ArchetypeIdx = archetype.Index,
//                EntityIdx = idx,
//                Version = version
//            };
//        }

//        public T GetComponent<T>(Entity entity)
//            where T : new()
//        {
//            this.ThrowIfEntityNotMatch(entity);

//            return !this.componentArrays.TryGetValue(TypeIndex<T>.Index, out IComponentArray? array)
//                ? throw new ArgumentException("Attempted to get component that is not present in archetype")
//                : ((ComponentArray<T>)array)[entity.EntityIdx];
//        }

//        internal T GetComponent<T>(int index)
//            where T : new() 
//            => !this.componentArrays.TryGetValue(TypeIndex<T>.Index, out IComponentArray? array)
//                ? throw new ArgumentException("Attempted to get component that is not present in archetype")
//                : ((ComponentArray<T>)array)[index];

//        public bool TryGetComponent<T>(Entity entity, out T? component)
//            where T : new()
//        {
//            this.ThrowIfEntityNotMatch(entity);

//            if (!this.componentArrays.TryGetValue(TypeIndex<T>.Index, out IComponentArray? array))
//            {
//                component = default;
//                return false;
//            }

//            component = ((ComponentArray<T>)array)[entity.EntityIdx];
//            return true;
//        }

//        public void SetComponent<T>(Entity entity, in T component)
//            where T : new()
//        {
//            this.ThrowIfEntityNotMatch(entity);

//            if (!this.componentArrays.TryGetValue(TypeIndex<T>.Index, out IComponentArray? array))
//                throw new ArgumentException("Attempted to set component that is not present in archetype");

//            ComponentArray<T> castArray = (ComponentArray<T>)array;

//            castArray[entity.EntityIdx] = component;
//        }

//        internal void SetComponent<T>(int index, in T component)
//            where T : new()
//        {
//            if (!this.componentArrays.TryGetValue(TypeIndex<T>.Index, out IComponentArray? array))
//                throw new ArgumentException("Attempted to set component that is not present in archetype");

//            ComponentArray<T> castArray = (ComponentArray<T>)array;

//            castArray[index] = component;
//        }

//        public bool TrySetComponent<T>(Entity entity, in T component)
//            where T : new()
//        {
//            this.ThrowIfEntityNotMatch(entity);

//            if (!this.componentArrays.TryGetValue(TypeIndex<T>.Index, out IComponentArray? array))
//                return false;

//            ComponentArray<T> castArray = (ComponentArray<T>)array;

//            castArray[entity.EntityIdx] = component;

//            return true;
//        }

//        public void ThrowIfEntityNotMatch(Entity entity)
//        {
//            if (!entity.MatchesArchetype(this))
//                throw new ArgumentException($"Entity {entity.EntityIdx} does not belong to archetype {this.Index} of world {this.world.Index}");
//        }

//        public override bool Equals(object? obj) 
//            => obj is Archetype archetype && this.MatchesArchetype(archetype);

//        public override int GetHashCode()
//            => this.componentArrays.GetHashCode();

//        public bool Equals(Archetype? other) => other is not null && this.MatchesArchetype(other);
//    }

//    public static partial class Extensions
//    {
//        public static bool MatchesArchetype(this Entity entity, in Archetype archetype)
//            => entity.WorldIdx == archetype.world.Index && entity.ArchetypeIdx == archetype.Index;

//        public static bool MatchesArchetype(this Archetype archetype, in Archetype other)
//        {
//            if (archetype.componentArrays.Count != other.componentArrays.Count)
//                return false;
//            foreach (ulong key in archetype.componentArrays.Keys)
//            {
//                if (!other.componentArrays.ContainsKey(key))
//                    return false;
//            }

//            foreach (ulong key in other.componentArrays.Keys)
//            {
//                if (!archetype.componentArrays.ContainsKey(key))
//                    return false;
//            }

//            return true;
//        }
//    }
//}
