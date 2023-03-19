namespace Xiphos.Utilities;

public readonly unsafe struct View<T>
    where T : unmanaged
{
    internal readonly T* _ptr;
    internal readonly ulong _offset;
    internal readonly ulong _length;

    public ulong Length => this._length;

    public ref T this[ulong index]
    {
        get
        {
            if (index < this._length) return ref this._ptr[index];
            throw new IndexOutOfRangeException();
        }
    }

    public ref T this[long index]
    {
        get
        {
            if (index < (long)this._length) return ref this._ptr[index];
            throw new IndexOutOfRangeException();
        }
    }

    public ref T this[uint index]
    {
        get
        {
            if (index < this._length)
                return ref this._ptr[index];
            throw new IndexOutOfRangeException();
        }
    }

    public ref T this[int index]
    {
        get
        {
            if (index < (long)this._length) return ref this._ptr[index];
            throw new IndexOutOfRangeException();
        }
    }

    public View(void* ptr, ulong offset, ulong length)
    {
        this._ptr = (T*)ptr;
        this._offset = offset;
        this._length = length;
    }

    public Span<T> AsSpan()
        => new(this._ptr, (int)this._length);

    public ReadOnlyView<T> AsReadOnly()
        => new(this._ptr, this._offset, this._length);

    public static implicit operator ReadOnlyView<T>(View<T> view) => view.AsReadOnly();
}

public readonly unsafe struct ReadOnlyView<T>
    where T : unmanaged
{
    internal readonly T* _ptr;
    internal readonly ulong _offset;
    internal readonly ulong _length;

    public ulong Length => this._length;



    public ref readonly T this[ulong index]
    {
        get
        {
            if (index < this._length) return ref this._ptr[index];
            throw new IndexOutOfRangeException();
        }
    }

    public ref readonly T this[long index]
    {
        get
        {
            if (index < (long)this._length) return ref this._ptr[index];
            throw new IndexOutOfRangeException();
        }
    }

    public ref readonly T this[uint index]
    {
        get
        {
            if (index < this._length)
                return ref this._ptr[index];
            throw new IndexOutOfRangeException();
        }
    }

    public ref readonly T this[int index]
    {
        get
        {
            if (index < (long)this._length) return ref this._ptr[index];
            throw new IndexOutOfRangeException();
        }
    }

    public ReadOnlyView(void* ptr, ulong offset, ulong length)
    {
        this._ptr = (T*)ptr;
        this._offset = offset;
        this._length = length;
    }

    public Span<T> AsSpan()
        => new(this._ptr, (int)this._length);
}
