//using System.Collections;

//namespace Xiphos.ECS2
//{
//    interface IEntityCache { }

//    interface IEntityCache<T> : IEntityCache 
//    {
//        public bool SlotEntity(T entity, out EntityIndex index);

//        public bool CreateEntity(out EntityIndex index);

//        public void FreeEntity(EntityIndex index);

//        public bool GetEntity(EntityIndex index, [NotNullWhen(true)] out T? entity);

//        public void SetEntity(EntityIndex index, in T entity);
//    }

//    public class EntityCache<T> : IEntityCache<T>, IEnumerable<EntityIndex>
//        where T : IEntity
//    {
//        readonly T[] cache;
//        readonly int[] locks;
//        readonly int[] versions;
//        int lastIndex;
//        int count;
//        readonly int size;
//        readonly ConcurrentQueue<int> indexQueue;

//        public int Count => Volatile.Read(ref this.count);

//        public EntityCache(int size)
//            => (this.cache, this.locks, this.versions, this.lastIndex, this.count, this.size, this.indexQueue) = (new T[size], new int[size], new int[size], -1, 0, size, new());

//        public bool SlotEntity(T entity, out EntityIndex index)
//        {
//            if (!this.indexQueue.TryDequeue(out int idx))
//                idx = Interlocked.Increment(ref this.lastIndex);

//            if (idx >= this.size)
//            {
//                index = default;
//                return false;
//            }

//            while (true)
//                if (this.LockEntity(idx))
//                    break;

//            this.versions[idx]++;
//            int vers = this.versions[idx];

//            this.cache[idx] = entity;

//            index = new() { index = idx, version = vers };

//            Interlocked.Increment(ref this.count);

//            this.UnlockEntity(idx);

//            return true;
//        }

//        public bool CreateEntity(out EntityIndex index)
//        {
//            if (!this.indexQueue.TryDequeue(out int idx))
//                idx = Interlocked.Increment(ref this.lastIndex);

//            if (idx >= this.size)
//            {
//                index = default;
//                return false;
//            }

//            while (true)
//                if (this.LockEntity(idx))
//                    break;
//                else
//                {
//                    index = default;
//                    return false;
//                }

//            this.versions[idx]++;
//            int vers = this.versions[idx];

//            index = new() { index = idx, version = vers };

//            Interlocked.Increment(ref this.count);

//            this.UnlockEntity(idx);

//            return true;
//        }

//        public void FreeEntity(EntityIndex index)
//        {
//            while (true)
//                if (this.LockEntity(index.index))
//                    break;

//            this.versions[index.index]++;

//            this.indexQueue.Enqueue(index.index);

//            Interlocked.Decrement(ref this.count);

//            this.UnlockEntity(index.index);
//        }

//        public bool GetEntity(EntityIndex index, [NotNullWhen(true)] out T? entity)
//        {
//            if (Environment.CurrentManagedThreadId <= 0)
//            {
//                Console.WriteLine("THREAD ID OF <= 0 REACHED");
//                throw new Exception("THREAD ID OF <= 0 REACHED");
//            }

//            while (true)
//                if (this.LockEntity(index.index))
//                    break;

//            if (index.version != this.versions[index.index])
//            {
//                this.UnlockEntity(index.index);
//                entity = default;
//                return false;
//            }

//            entity = this.cache[index.index];
//            return true;
//        }

//        public void SetEntity(EntityIndex index, in T entity)
//        {
//            if (Environment.CurrentManagedThreadId <= 0)
//            {
//                Console.WriteLine("THREAD ID OF <= 0 REACHED");
//                throw new Exception("THREAD ID OF <= 0 REACHED");
//            }

//            if (Volatile.Read(ref this.locks[index.index]) == Environment.CurrentManagedThreadId)
//            {
//                this.cache[index.index] = entity;

//                this.UnlockEntity(index.index);
//            }
//        }

//        bool LockEntity(int index) 
//            => Interlocked.CompareExchange(ref this.locks[index], Environment.CurrentManagedThreadId, 0) == 0;

//        bool UnlockEntity(int index)
//        {
//            int threadId = Environment.CurrentManagedThreadId;
//            return Interlocked.CompareExchange(ref this.locks[index], 0, threadId) == threadId;
//        }

//        public IEnumerator<EntityIndex> GetEnumerator() => new Enumerator(-1, -1, this);
//        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(-1, -1, this);

//        public struct Enumerator : IEnumerator<EntityIndex>
//        {
//            internal int index;
//            internal int version;
//            internal readonly EntityCache<T> cache;

//            public Enumerator(int index, int version, EntityCache<T> cache)
//                => (this.index, this.version, this.cache) = (index, version, cache);

//            public EntityIndex Current => new() { index = this.index, version = this.version };
//            object IEnumerator.Current => new EntityIndex() { index = this.index, version = this.version };

//            public void Dispose() { }
//            public bool MoveNext()
//            {
//                var count = this.cache.Count;
//                do
//                {
//                    this.index++;
//                    if (this.index >= count)
//                        return false;
//                } while ((this.version = Volatile.Read(ref this.cache.versions[this.index])) % 2 == 0);
//                return true;
//            }
//            public void Reset() => this.index = -1;
//        }
//    }
//}
