using System.Runtime.CompilerServices;

namespace Xiphos.Utilities;

/// <summary>
/// Contains utility extensions for converting byte arrays into data types
/// </summary>
public static partial class ByteConversionExtensions
{
    /// <summary>
    /// Reinterpret a slice of bytes to any type
    /// </summary>
    /// <typeparam name="T">Type to reinterpret bytes to</typeparam>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static unsafe T CastContents<T>(this ReadOnlyMemory<byte> bytes)
        where T : unmanaged
    {
        int size = Unsafe.SizeOf<T>();
        if (size > 256)
            throw new ArgumentException($"Type {typeof(T).Name} is too large to interpret cast");
        byte* ptr = stackalloc byte[size];
        Span<byte> span = new(ptr, size);
        bytes.Span.CopyTo(span);
        return Unsafe.As<byte, T>(ref *ptr);
    }

    /// <summary>
    /// Reinterpret a slice of bytes to any type
    /// </summary>
    /// <typeparam name="T">Type to reinterpret bytes to</typeparam>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static unsafe T Cast<T>(this Memory<byte> bytes)
        where T : unmanaged
        => ((ReadOnlyMemory<byte>)bytes).CastContents<T>();

    /// <summary>
    /// Reinterpret a slice of bytes to any type
    /// </summary>
    /// <typeparam name="T">Type to reinterpret bytes to</typeparam>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static unsafe T CastContents<T>(this ReadOnlySpan<byte> bytes)
        where T : unmanaged
    {
        int size = Unsafe.SizeOf<T>();
        if (size > 256)
            throw new ArgumentException($"Type {typeof(T).Name} is too large to interpret cast");
        byte* ptr = stackalloc byte[size];
        Span<byte> span = new(ptr, size);
        bytes.CopyTo(span);
        return Unsafe.As<byte, T>(ref *ptr);
    }

    /// <summary>
    /// Reinterpret a slice of bytes to any type
    /// </summary>
    /// <typeparam name="T">Type to reinterpret bytes to</typeparam>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static unsafe T Cast<T>(this Span<byte> bytes)
        where T : unmanaged
        => ((ReadOnlySpan<byte>)bytes).CastContents<T>();

    /// <summary>
    /// Reinterpret a ulong as any unmanaged type that is up to 64 bits large
    /// </summary>
    /// <typeparam name="T">Type to reinterpret to</typeparam>
    /// <param name="primitive"></param>
    /// <returns></returns>
    internal static unsafe TTo Cast<TFrom, TTo>(this TFrom @base)
        where TFrom : unmanaged
        where TTo : unmanaged
    {
        if (Unsafe.SizeOf<TTo>() > Unsafe.SizeOf<TFrom>())
            throw new ArgumentException("Destination size larger than source size");
        ref TTo newResult = ref Unsafe.As<TFrom, TTo>(ref @base);
        return newResult;
    }

    internal static unsafe TTo CreateFromBytes<TFrom, TTo>(this TFrom @base)
        where TFrom : unmanaged
        where TTo : unmanaged
    {
        TTo result = default!;
        int size = Math.Min(Unsafe.SizeOf<TFrom>(), Unsafe.SizeOf<TTo>());
        Unsafe.CopyBlock(ref Unsafe.As<TTo, byte>(ref result), ref Unsafe.As<TFrom, byte>(ref @base), (uint)size);
        return result;
    }

    //internal static unsafe ReadOnlySpan<byte> GetBytes<T>(this ref T @base)
    //    where T : unmanaged
    //    => new(Unsafe.AsPointer(ref @base), Unsafe.SizeOf<T>());

    internal static unsafe Span<TTo> As<TFrom, TTo>(this Span<TFrom> span)
        where TFrom : unmanaged
        where TTo : unmanaged
        => MemoryMarshal.Cast<TFrom, TTo>(span);

    internal static unsafe ReadOnlySpan<TTo> As<TFrom, TTo>(this ReadOnlySpan<TFrom> span)
        where TFrom : unmanaged
        where TTo : unmanaged
        => MemoryMarshal.Cast<TFrom, TTo>(span);

    internal static unsafe Span<TTo> As<TTo>(this Memory<byte> memory)
        where TTo : unmanaged
    {
        Span<byte> bytes = memory.Span;
        return MemoryMarshal.Cast<byte, TTo>(bytes);
    }

    internal static unsafe ReadOnlySpan<TTo> As<TTo>(this ReadOnlyMemory<byte> memory)
        where TTo : unmanaged
    {
        ReadOnlySpan<byte> bytes = memory.Span;
        return MemoryMarshal.Cast<byte, TTo>(bytes);
    }
}
