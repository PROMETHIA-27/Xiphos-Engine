using System.Runtime.CompilerServices;

namespace Xiphos.ECS2
{
    public readonly unsafe struct DynamicBitflags : IDisposable, IEquatable<DynamicBitflags>
    {
        private readonly IntPtr ptr;
        private readonly ulong size;

        public DynamicBitflags(ulong sizeInBytes)
        {
            this.size = sizeInBytes * 8;
            this.ptr = Marshal.AllocHGlobal((IntPtr)sizeInBytes);
            Unsafe.InitBlock((void*)this.ptr, 0, (uint)sizeInBytes);
        }

        public ulong Size => this.size;
        public ulong ByteSize => this.size / 8;

        public int this[ulong bit]
        {
            get
            {
                if (bit >= this.size)
                    throw new IndexOutOfRangeException();
                ulong idx = bit / 8;
                byte* ptr = (byte*)this.ptr;
                int value = (ptr[idx] & (1 << ((int)bit % 8))) >> ((int)bit % 8);
                return value;
            }

            set
            {
                if (value is not (0 or 1))
                    throw new ArgumentException("Value must be 0 or 1");
                if (bit >= this.size)
                    throw new IndexOutOfRangeException();
                ulong idx = bit / 8;
                byte offset = (byte)(bit % 8);
                byte* ptr = (byte*)this.ptr;
                ptr[idx] &= (byte)~(1 << offset);
                ptr[idx] |= (byte)(value << offset);
            }
        }

        public bool TryGet(ulong bit, out int value)
        {
            if (bit >= this.size)
            {
                value = default;
                return false;
            }
            value = this[bit];
            return true;
        }

        public bool TrySet(ulong bit, int value)
        {
            if (bit >= this.size)
                return false;
            this[bit] = value;
            return true;
        }

        public int GetOrFalse(ulong bit)
        {
            if (bit >= this.size)
                return 0;
            return this[bit];
        }

        public byte? GetByteOrNull(ulong @byte)
        {
            if (@byte >= this.ByteSize)
                return 0;
            return ((byte*)this.ptr)[@byte];
        }

        public override int GetHashCode()
        {
            byte* ptr = (byte*)this.ptr;
            int hash = ptr[0].GetHashCode();
            for (ulong i = 1; i < this.size; i++)
            {
                if (ptr[i] != 0)
                    hash = HashCode.Combine(ptr[i]);
            }

            return hash;
        }

        public bool Equals(DynamicBitflags other)
        {
            (DynamicBitflags lowest, DynamicBitflags highest) = this.size > other.size ? (other, this) : (this, other);
            for (ulong i = 0; i < highest.ByteSize; i++)
            {
                if (highest.GetByteOrNull(i) != lowest.GetByteOrNull(i))
                    return false;
            }

            return true;
        }

        public void Dispose()
            => Marshal.FreeHGlobal(this.ptr);
    }
}
