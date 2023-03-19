//namespace Xiphos.ECS2
//{
//    internal unsafe struct EntityStateBuffer
//    {
//        private readonly EntityStateCache cache1;
//        private readonly EntityStateCache cache2;
//        private readonly ulong cacheSize;
//        private readonly ulong activeCache;
//        private readonly Dictionary<ulong, IntPtr> commandBuffers;

//        public EntityStateBuffer(int numberComponents, ulong cacheSize)
//        {
//            this.cache1 = new(numberComponents, cacheSize);
//            this.cache2 = new(numberComponents, cacheSize);
//            this.cacheSize = cacheSize;
//            this.activeCache = 0;
//            this.commandBuffers = new(numberComponents);
//        }

//        //public void CommandSet<T>(ulong index, T newValue)
//        //{
//        //    var buffPtr = this.commandBuffers[TypeIndex<T>.Index];
//        //    ref CommandBufferHeader header = ref GetHeader(buffPtr);
//        //    var buffer = new Span<Command<T>>((void*)buffPtr, (int)this.cacheSize);
//        //    if (index >= this.cacheSize)
//        //        throw new ArgumentOutOfRangeException("index");
//        //    SpinWait wait = default;
//        //    while (Interlocked.CompareExchange(ref buffer[(int)index].@lock, 1, 0) != 0)
//        //        wait.SpinOnce();
//        //    buffer[(int)index] = new() { newValue = newValue, valid = 1 };
//        //    Interlocked.Exchange(ref buffer[(int)index].@lock, 0);
//        //}

//        internal void ApplyCommandBuffer(ulong typeIdx)
//        {
//            (IntPtr ptr, ulong cacheSize) = this.GetActiveCache().componentCaches[typeIdx];
//            byte* cachePtr = (byte*)ptr;
//            IntPtr buffPtr = this.commandBuffers[typeIdx];
//            ref CommandBufferHeader header = ref GetHeader(buffPtr);
//            byte* bufferPtr = (byte*)buffPtr;

//            ulong elementSize = cacheSize / this.cacheSize;
//            ulong commandSize = header.commandSize;
//            ulong valOffset = header.newValueOffset;
//            ulong validOffset = header.validOffset;

//            for (ulong i = 0; i < this.cacheSize; i++)
//            {
//                byte* command = bufferPtr + (i * commandSize);
//                if (*(ulong*)(command + validOffset) != 1)
//                    continue;
//                byte* commandValuePtr = command + valOffset;
//                Unsafe.CopyBlock(cachePtr + (i * elementSize), commandValuePtr, (uint)elementSize);
//                *(ulong*)(command + validOffset) = 0;
//            }
//            //this.ClearCommandBuffer(typeIdx);
//        }

//        //internal void ApplyCommandBuffer<T>()
//        //{
//        //    var cache = this.GetActiveCache().GetCache<T>();
//        //    var buffer = this.GetCommandSpan<T>();
//        //    for (int i = 0; i < buffer.Length; i++)
//        //    {
//        //        ref var command = ref buffer[i];
//        //        if (command.valid != 1)
//        //            continue;
//        //        cache[i] = command.newValue;
//        //        command.valid = 0;
//        //    }
//        //    //this.ClearCommandBuffer<T>();
//        //}

//        internal void ApplyCommands()
//        {
//            foreach (ulong typeIdx in this.GetActiveCache().componentCaches.Keys)
//                this.ApplyCommandBuffer(typeIdx);
//        }

//        //internal void ClearCommandBuffer(ulong typeIdx)
//        //{
//        //    var buffPtr = this.commandBuffers[typeIdx];
//        //    ref var header = ref GetHeader(buffPtr);

//        //    for (ulong i = 0; i < this.cacheSize; i++)
//        //    {

//        //    }
//        //}

//        //internal void ClearCommandBuffer<T>()
//        //{
//        //    var buffPtr = this.commandBuffers[TypeIndex<T>.Index];
//        //    ref var header = ref GetHeader(buffPtr);
//        //    Interlocked.Exchange(ref header.count, 0);
//        //}

//        internal EntityStateCache GetActiveCache()
//            => this.activeCache == 0 ? this.cache1 : this.cache2;

//        internal Span<Command<T>> GetCommandSpan<T>()
//        {
//            IntPtr buffPtr = this.commandBuffers[TypeIndex<T>.Index];
//            ref CommandBufferHeader header = ref GetHeader(buffPtr);
//            return new Span<Command<T>>((void*)buffPtr, (int)this.cacheSize);
//        }

//        internal static ref CommandBufferHeader GetHeader(IntPtr ptr)
//            => ref Unsafe.AsRef<CommandBufferHeader>((void*)(ptr - sizeof(CommandBufferHeader)));

//        internal static ulong SizeOfCommand<T>()
//            => (ulong)Unsafe.SizeOf<Command<T>>();

//        internal struct CommandBufferHeader
//        {
//            public ulong commandSize;
//            public ulong newValueOffset;
//            public ulong validOffset;
//        }

//        internal struct Command<T>
//        {
//            public ulong @lock;
//            public ulong valid;
//            public ulong blockEnd;
//            public ulong blockStart;
//            public T newValue;
//        }
//    }
//}
