namespace Xiphos.Monads;

#pragma warning disable IDE1006 // Naming Styles
public struct Vec2<T>
    where T : unmanaged
{
    public T x { get; set; }
    public T y { get; set; }

    public T this[int index]
    {
        get => index switch
        {
            0 => this.x,
            1 => this.y,
            _ => throw new IndexOutOfRangeException(),
        };
        set
        {
            switch (index)
            {
                case 0:
                    this.x = value;
                    break;
                case 1:
                    this.y = value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }

    public Vec2(T x, T y)
        => (this.x, this.y) = (x, y);

    public void Deconstruct(out T x, out T y)
        => (x, y) = (this.x, this.y);

    public static implicit operator Vec2<T>((T x, T y) t)
        => new(t.x, t.y);

    public void Map(Action<T> map)
    {
        map(this.x);
        map(this.y);
    }

    public unsafe void Map(delegate*<T, void> map)
    {
        map(this.x);
        map(this.y);
    }

    public Vec2<U> Map<U>(Func<T, U> map)
        where U : unmanaged
        => (map(this.x), map(this.y));

    public unsafe Vec2<U> Map<U>(delegate*<T, U> map)
        where U : unmanaged
        => (map(this.x), map(this.y));
}

public struct Vec3<T>
    where T : unmanaged
{
    public T x { get; set; }
    public T y { get; set; }
    public T z { get; set; }

    public T this[int index]
    {
        get => index switch
        {
            0 => this.x,
            1 => this.y,
            2 => this.z,
            _ => throw new IndexOutOfRangeException(),
        };
        set
        {
            switch (index)
            {
                case 0:
                    this.x = value;
                    break;
                case 1:
                    this.y = value;
                    break;
                case 2:
                    this.z = value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }

    public Vec3(T x, T y, T z)
        => (this.x, this.y, this.z) = (x, y, z);

    public Vec3(Vec2<T> xy, T z)
        => ((this.x, this.y), this.z) = (xy, z);

    public void Deconstruct(out T x, out T y, out T z)
        => (x, y, z) = (this.x, this.y, this.z);

    public static implicit operator Vec3<T>((T x, T y, T z) t)
        => new(t.x, t.y, t.z);

    public static implicit operator Vec3<T>((Vec2<T> xy, T z) t)
        => new(t.xy, t.z);

    public void Map(Action<T> map)
    {
        map(this.x);
        map(this.y);
        map(this.z);
    }

    public unsafe void Map(delegate*<T, void> map)
    {
        map(this.x);
        map(this.y);
        map(this.z);
    }

    public Vec3<U> Map<U>(Func<T, U> map)
        where U : unmanaged
        => (map(this.x), map(this.y), map(this.z));

    public unsafe Vec3<U> Map<U>(delegate*<T, U> map)
        where U : unmanaged
        => (map(this.x), map(this.y), map(this.z));
}

public struct Vec4<T>
    where T : unmanaged
{
    public T x { get; set; }
    public T y { get; set; }
    public T z { get; set; }
    public T w { get; set; }

    public T this[int index]
    {
        get => index switch
        {
            0 => this.x,
            1 => this.y,
            2 => this.z,
            3 => this.w,
            _ => throw new IndexOutOfRangeException(),
        };
        set
        {
            switch (index)
            {
                case 0:
                    this.x = value;
                    break;
                case 1:
                    this.y = value;
                    break;
                case 2:
                    this.z = value;
                    break;
                case 3:
                    this.w = value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }

    public Vec4(T x, T y, T z, T w)
        => (this.x, this.y, this.z, this.w) = (x, y, z, w);

    public Vec4(Vec3<T> xyz, T w)
        => ((this.x, this.y, this.z), this.w) = (xyz, w);

    public void Deconstruct(out T x, out T y, out T z, out T w)
        => (x, y, z, w) = (this.x, this.y, this.z, this.w);

    public static implicit operator Vec4<T>((T x, T y, T z, T w) t) 
        => new(t.x, t.y, t.z, t.w);

    public static implicit operator Vec4<T>((Vec3<T> xyz, T w) t)
        => new(t.xyz, t.w);

    public void Map(Action<T> map)
    {
        map(this.x);
        map(this.y);
        map(this.z);
        map(this.w);
    }

    public unsafe void Map(delegate*<T, void> map)
    {
        map(this.x);
        map(this.y);
        map(this.z);
        map(this.w);
    }

    public Vec4<U> Map<U>(Func<T, U> map)
        where U : unmanaged
        => (map(this.x), map(this.y), map(this.z), map(this.w));

    public unsafe Vec4<U> Map<U>(delegate*<T, U> map)
        where U : unmanaged
        => (map(this.x), map(this.y), map(this.z), map(this.w));
}
#pragma warning restore IDE1006 // Naming Styles
