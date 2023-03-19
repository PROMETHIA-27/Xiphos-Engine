namespace Xiphos.Threading
{
    internal struct AtomicBool
    {
        private long value;

        public AtomicBool(bool value)
            => this.value = value ? 1 : 0;

        public bool Value
        {
            get => Interlocked.Read(ref this.value) != 0;

            set => Interlocked.Exchange(ref this.value, value ? 1 : 0);
        }

        public static implicit operator bool(AtomicBool atomic) => atomic.Value;
        public static implicit operator AtomicBool(bool @bool) => new(@bool);
    }
}
