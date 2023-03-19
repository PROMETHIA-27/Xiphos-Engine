using System.Runtime.CompilerServices;

namespace Xiphos.ECS2
{
    public readonly unsafe struct EntityStateCache : IDisposable
    {
        internal readonly Dictionary<ulong, (IntPtr ptr, ulong size)> componentCaches;
        internal readonly DynamicBitflags signature;
        internal readonly ulong cacheSize;
        internal readonly Dictionary<ulong, IntPtr> commandBuffers;

        //public EntityStateCache DeepCopy()
        //{
        //    EntityStateCache newCache = new(this.componentCaches.Count, this.cacheSize);
        //    foreach (var (index, cache) in this.componentCaches)
        //    {
        //        void* ptr = (void*)Marshal.AllocHGlobal((IntPtr)cache.size);
        //        Unsafe.CopyBlock(ptr, (void*)cache.ptr, (uint)cache.size);
        //        newCache.componentCaches.Add(index, ((IntPtr)ptr, cache.size));
        //    }
        //    newCache.signature = this.signature;
        //    return newCache;
        //}

        public ReadOnlySpan<T> GetReadonlyCache<T>()
            => this.GetCache<T>();

        internal EntityStateCache(int numberComponents, ulong cacheSize)
            => (this.componentCaches, this.signature, this.cacheSize, this.commandBuffers) = (new(numberComponents), default, cacheSize, new(numberComponents));

        internal EntityStateCache(Dictionary<ulong, (IntPtr ptr, ulong size)> caches, DynamicBitflags signature, ulong cacheSize, Dictionary<ulong, IntPtr> buffers)
            => (this.componentCaches, this.signature, this.cacheSize, this.commandBuffers) = (caches, signature, cacheSize, buffers);

        internal ulong SizeOfCache<T>()
            => (ulong)Unsafe.SizeOf<T>() * this.cacheSize;

        //internal void AddCache<T>()
        //{
        //    IntPtr ptr = Marshal.AllocHGlobal((IntPtr)this.SizeOfCache<T>());
        //    this.componentCaches.Add(TypeIndex<T>.Index, (ptr, this.SizeOfCache<T>()));
        //    IntPtr bufferPtr = Marshal.AllocHGlobal((IntPtr)((ulong)sizeof(CommandBufferHeader) + (SizeOfCommand<T>() * this.cacheSize)));
        //    Command<T> temp = default;
        //    *(CommandBufferHeader*)bufferPtr = new()
        //    {
        //        commandSize = SizeOfCommand<T>(),
        //        newValueOffset = (nuint)Unsafe.AsPointer(ref temp.newValue) - (nuint)Unsafe.AsPointer(ref temp),
        //        validOffset = (nuint)Unsafe.AsPointer(ref temp.valid) - (nuint)Unsafe.AsPointer(ref temp),
        //    };
        //    bufferPtr += sizeof(CommandBufferHeader);
        //    this.commandBuffers.Add(TypeIndex<T>.Index, bufferPtr);
        //}

        //internal void AddCache<T>(Span<T> components)
        //{
        //    this.AddCache<T>();
        //    this.CopySpanToCache(components);
        //}

        internal void CopySpanToCache<T>(Span<T> span)
        {
            (IntPtr ptr, ulong size) = this.componentCaches[TypeIndex<T>.Index];
            Span<T> cacheSpan = new((void*)ptr, (int)size);
            span.CopyTo(cacheSpan);
        }

        internal void CopyToCache(EntityStateCache cache)
        {
            foreach ((ulong _, (IntPtr ptr, ulong size) cachePtr) in cache.componentCaches)
                Marshal.FreeHGlobal((IntPtr)(void*)cachePtr.ptr);
            cache.componentCaches.Clear();
            foreach ((ulong typeIdx, (IntPtr ptr, ulong size) cachePtr) in this.componentCaches)
            {
                IntPtr newPtr = Marshal.AllocHGlobal((IntPtr)cachePtr.size);
                cache.componentCaches.Add(typeIdx, (newPtr, cachePtr.size));
                Unsafe.CopyBlock((void*)cachePtr.ptr, (void*)newPtr, (uint)cachePtr.size);
            }
        }

        internal Span<T> GetCache<T>()
        {
            void* ptr = (void*)this.componentCaches[TypeIndex<T>.Index].ptr;

            return new(ptr, (int)this.cacheSize);
        }

        public void Dispose() //TODO: Dispose of memory
        {

        }

        //public class Builder
        //{
        //    EntityStateCache cache;
        //    bool compiled;

        //    public Builder(int componentCount, ulong cacheSize)
        //        => this.cache = new(componentCount, cacheSize);

        //    public Builder AddComponent<T>()
        //    {
        //        if (!this.compiled)
        //        {
        //            this.cache.AddCache<T>();
        //            return this;
        //        }
        //        else
        //            throw new Exception("Cannot modify a compiled EntityStateCache");
        //    }

        //    public Builder AddComponent<T>(Span<T> components)
        //    {
        //        if (!this.compiled)
        //        {
        //            this.cache.AddCache(components);
        //            return this;
        //        }
        //        else
        //            throw new Exception("Cannot modify a compiled EntityStateCache");
        //    }

        //    public EntityStateCache Compile()
        //    {
        //        this.compiled = true;
        //        var temp = this.cache;
        //        var highest = 0UL;
        //        foreach (var index in temp.componentCaches.Keys)
        //            if (index > highest)
        //                highest = index;
        //        var flagsLength = (ulong)Math.Ceiling((double)(highest + 1) / 8);
        //        DynamicBitflags newSignature = new(flagsLength);
        //        foreach (var (index, _) in temp.componentCaches)
        //            newSignature[index] = 1;
        //        this.cache = default;
        //        return new(temp.componentCaches, newSignature, temp.cacheSize, temp.commandBuffers);
        //    }
        //}
    }
}
