namespace Xiphos.Threading
{
    internal struct AtomicLong
    {
        private long value;

        public AtomicLong(long value)
            => this.value = value;

        public long Value
        {
            get => Interlocked.Read(ref this.value);

            set => Interlocked.Exchange(ref this.value, value);
        }

        public static implicit operator long(AtomicLong atomic) => atomic.Value;
        public static implicit operator AtomicLong(long @long) => new(@long);
    }
}
