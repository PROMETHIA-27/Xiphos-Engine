using System.Runtime.CompilerServices;

namespace Xiphos.Utilities
{
    public readonly unsafe struct Ptr<T>
        where T : unmanaged
    {
        internal readonly void* ptr;

        public ref T this[ulong index]
            => ref Unsafe.AsRef<T>((byte*)this.ptr + (index * (ulong)Unsafe.SizeOf<T>()));

        public ref T this[long index]
            => ref Unsafe.AsRef<T>((byte*)this.ptr + (index * Unsafe.SizeOf<T>()));

        public ref T this[uint index]
            => ref Unsafe.AsRef<T>((byte*)this.ptr + (index * (ulong)Unsafe.SizeOf<T>()));

        public ref T this[int index]
            => ref Unsafe.AsRef<T>((byte*)this.ptr + (index * Unsafe.SizeOf<T>()));

        public ref T Val => ref Unsafe.AsRef<T>(this.ptr);

        public Ptr(void* ptr)
            => this.ptr = ptr;

        public Span<T> AsSpan(int size)
            => new(this.ptr, size);
    }
}
