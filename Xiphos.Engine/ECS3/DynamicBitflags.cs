using System.Runtime.CompilerServices;

namespace Xiphos.ECS3;
#pragma warning disable IDE0032 // Use auto property
public readonly unsafe struct DynamicBitflags : IDisposable, IEquatable<DynamicBitflags>
{
    private readonly IntPtr _ptr;
    private readonly ulong _size;

    public DynamicBitflags(ulong sizeInBytes)
    {
        this._size = sizeInBytes * 8;
        this._ptr = Marshal.AllocHGlobal((IntPtr)sizeInBytes);
        Unsafe.InitBlock((void*)this._ptr, 0, (uint)sizeInBytes);
    }

    public ulong Size => this._size;
    public ulong ByteSize => this._size / 8;

    public int this[ulong bit]
    {
        get
        {
            if (bit >= this._size)
                throw new IndexOutOfRangeException();
            ulong idx = bit / 8;
            byte* ptr = (byte*)this._ptr;
            int value = (ptr[idx] & (1 << ((int)bit % 8))) >> ((int)bit % 8);
            return value;
        }

        set
        {
            if (value is not (0 or 1))
                throw new ArgumentException("Value must be 0 or 1");
            if (bit >= this._size)
                throw new IndexOutOfRangeException();
            ulong idx = bit / 8;
            byte offset = (byte)(bit % 8);
            byte* ptr = (byte*)this._ptr;
            ptr[idx] &= (byte)~(1 << offset);
            ptr[idx] |= (byte)(value << offset);
        }
    }

    public Either<AccessError, int> Get(ulong bit)
        => bit < this._size ? new(this[bit]) : new(AccessError.IndexOutOfRange);

    public Maybe<AccessError> Set(ulong bit, int value)
    {
        if (bit >= this._size)
            return new(AccessError.IndexOutOfRange);
        this[bit] = value;
        return new();
    }

    public int GetOrFalse(ulong bit)
        => this.Get(bit) switch
        {
            (Either.Right, var value) => value,
            _ => 0,
        };

    public byte? GetByteOrNull(ulong @byte) 
        => @byte < this.ByteSize ? ((byte*)this._ptr)[@byte] : null;

    public override int GetHashCode()
    {
        byte* ptr = (byte*)this._ptr;
        int hash = ptr[0].GetHashCode();
        for (ulong i = 1; i < this._size; i++)
        {
            if (ptr[i] != 0)
                hash = HashCode.Combine(ptr[i]);
        }

        return hash;
    }

    public bool Equals(DynamicBitflags other)
    {
        (DynamicBitflags lowest, DynamicBitflags highest) = this._size > other._size ? (other, this) : (this, other);
        for (ulong i = 0; i < highest.ByteSize; i++)
        {
            if (highest.GetByteOrNull(i) != lowest.GetByteOrNull(i))
                return false;
        }

        return true;
    }

    public void Dispose()
        => Marshal.FreeHGlobal(this._ptr);

    public enum AccessError
    {
        IndexOutOfRange,
    }
}
