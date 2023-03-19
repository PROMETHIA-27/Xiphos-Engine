using System.Runtime.CompilerServices;

namespace Xiphos.Utilities;

public static class GltfExtensions
{
    public static uint ByteLength<T>(this Span<T> span)
        => (uint)Unsafe.SizeOf<T>() * (uint)span.Length;
    public static uint ByteLength<T>(this ReadOnlySpan<T> span)
        => (uint)Unsafe.SizeOf<T>() * (uint)span.Length;

    //public static Span<T> GetAccessor<T>(this Gltf gltf, byte[][] buffers, ulong index)
    //    where T : unmanaged
    //{
    //    Accessor? accessor = gltf.Accessors[index];
    //    BufferView? view = gltf.BufferViews[accessor.BufferView!.Value];
    //    Span<byte> byteSpan = buffers[view.Buffer].AsSpan(
    //        (accessor.ByteOffset + view.ByteOffset)..(accessor.ByteOffset + view.ByteOffset + view.ByteLength));
    //    return MemoryMarshal.Cast<byte, T>(byteSpan);
    //}

    public static Memory<byte> GetAccessor(this Gltf gltf, byte[][] buffers, ulong index)
    {
        Accessor accessor = gltf.Accessors[index];
        BufferView view = gltf.BufferViews[accessor.BufferView!.Value];
        return buffers[view.Buffer].AsMemory(
            (accessor.ByteOffset + view.ByteOffset)..(accessor.ByteOffset + view.ByteOffset + view.ByteLength));
    }

    //public static glTFLoader.Schema.Mesh GetMesh(this Gltf gltf, byte[][] buffers, ulong index)
    //{
    //    glTFLoader.Schema.Mesh meshData = gltf.Meshes[index];
    //    MeshPrimitive[]? meshPrims = new MeshPrimitive[meshData.Primitives.Length];
    //    for (int i = 0; i < meshPrims.Length; i++)
    //    {
    //        glTFLoader.Schema.MeshPrimitive primData = meshData.Primitives[i];

    //        ReadOnlyMemory<byte>? position = null;
    //        ReadOnlyMemory<byte>? normal = null;
    //        ReadOnlyMemory<byte>? tangent = null;
    //        ReadOnlyMemory<byte>? tex_coord0 = null;
    //        ReadOnlyMemory<byte>? tex_coord1 = null;
    //        ReadOnlyMemory<byte>? color = null;
    //        ReadOnlyMemory<byte>? joint = null;
    //        ReadOnlyMemory<byte>? weight = null;
    //        ReadOnlyMemory<byte> indices = null;

    //        if (primData.Attributes.TryGetValue("POSITION", out int posIdx))
    //            position = GetAccessor(gltf, buffers, (ulong)posIdx);
    //        if (primData.Attributes.TryGetValue("NORMAL", out int normalIdx))
    //            normal = GetAccessor(gltf, buffers, (ulong)normalIdx);

    //        indices = GetAccessor(gltf, buffers, (ulong)primData.Indices);

    //        meshPrims[i] = new()
    //        {
    //            Positions = position,
    //            Normals = normal,
    //            Indices = indices,
    //        };
    //    }

    //    return new()
    //    {
    //        Binaries = buffers,
    //        Primitives = meshPrims,
    //    };
    //}
}
