//namespace Xiphos.ECS2
//{
//    public sealed class BufferedConcurrentQueue<T>
//    {
//        private volatile uint low;
//        private volatile uint high;
//        private readonly uint size;
//        private readonly T[] array;
//        private readonly VolatileBool[] state;
//        private volatile int count;
//        internal uint Low => this.low;
//        internal uint High => this.high;

//        public int Count => this.count;

//        public BufferedConcurrentQueue(int powerOfTwo)
//        {
//            if (powerOfTwo <= 0)
//                throw new ArgumentOutOfRangeException(nameof(powerOfTwo));
//            if (powerOfTwo > 32)
//                throw new ArgumentOutOfRangeException(nameof(powerOfTwo));
//            this.size = (uint)Math.Pow(2, powerOfTwo);
//            this.array = new T[this.size];
//            this.state = new VolatileBool[this.size];
//            this.low = this.high = 0;
//        }

//        public bool TryEnqueue(T item)
//        {
//            if (Interlocked.Increment(ref this.count) > this.size)
//            {
//                _ = Interlocked.Decrement(ref this.count);
//                return false;
//            }

//            try
//            { }
//            finally
//            {
//                uint newHigh = Interlocked.Increment(ref this.high) - 1;
//                uint index = newHigh & (this.size - 1);
//                this.array[index] = item;
//                this.state[index] = new() { value = 1 };
//            }

//            return true;
//        }

//        public bool TryDequeue(out T result)
//        {
//            result = default!;
//            SpinWait spinWait = default;
//            uint localLow = this.low, localHigh = this.high;
//            while (localHigh - localLow > 0)
//            {
//                if (Interlocked.CompareExchange(ref this.low, localLow + 1, localLow) == localLow)
//                {
//                    uint index = localLow & (this.size - 1);
//                    while (Interlocked.CompareExchange(ref this.state[index].value, 0, 1) == 0)
//                        spinWait.SpinOnce();
//                    result = this.array[index];
//                    _ = Interlocked.Decrement(ref this.count);
//                    return true;
//                }

//                spinWait.SpinOnce();
//                localLow = this.low;
//                localHigh = this.high;
//            }

//            return false;
//        }

//        private struct VolatileBool
//        {
//            public volatile uint value;
//        }
//    }
//}
