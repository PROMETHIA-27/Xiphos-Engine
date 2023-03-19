using System.Runtime.CompilerServices;

namespace Xiphos.Utilities;

public static unsafe class Heap
{
    public static T* Create<T>(T value)
        where T : unmanaged
    {
        T* ptr = (T*)Marshal.AllocHGlobal(sizeof(T));
        *ptr = value;
        return ptr;
    }

    public static T* CreateArray<T>(T initialValue, ulong count)
        where T : unmanaged
    {
        T* ptr = (T*)Marshal.AllocHGlobal((IntPtr)((uint)sizeof(T) * count));
        for (ulong i = 0; i < count; i++)
            ptr[i] = initialValue;
        return ptr;
    }

    public static void Free<T>(T* ptr)
        where T : unmanaged
        => Marshal.FreeHGlobal((IntPtr)ptr);

    public static HeapBox CreateBox<T>(in T value)
        where T : unmanaged
    {
        T* ptr = (T*)Marshal.AllocHGlobal(sizeof(T));
        *ptr = value;
        HeapBox box = new() { Ptr = (IntPtr)ptr, Size = (uint)sizeof(T) };
        return box;
    }

    public static void FreeBox<T>(HeapBox box)
        where T : unmanaged
        => Marshal.FreeHGlobal(box.Ptr);

    public static void CopyBoxValue(HeapBox box, void* destination) 
        => Unsafe.CopyBlock(destination, (void*)box.Ptr, box.Size);
}

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct HeapBox
{
    internal uint Size { get; init; }
    internal IntPtr Ptr { get; init; }
}