namespace Xiphos.Graphics;

public struct StandardMeshFormat : ISerializationFormat
{
    public static string AssetCode => "MESH";
}

public unsafe readonly struct Mesh : IDisposable
{
    internal DeviceBuffer VertexBuffer { get; init; }
    internal DeviceBuffer IndexBuffer { get; init; }
    internal uint VertexCount { get; init; }
    internal uint IndexCount { get; init; }
    internal VertexElements Elements { get; init; }
    internal uint SerializedByteSize { get; init; }

    public void Dispose()
    {
        this.VertexBuffer.Dispose();
        this.IndexBuffer.Dispose();
    }

    [Flags]
    public enum VertexElements
    {
        None = 0,
        Position = 1,
        Normal = 2,
        Tangent = 4,
        TexCoord0 = 8,
        TexCoord1 = 16,
        Colors = 32,
        Joints = 64,
        Weights = 128,
        Indices = 256,
        All = 511,
    }

    public static int ElementCount { get; } = Enum.GetNames(typeof(VertexElements)).Length - 2;

    public static ImmutableArray<uint> ElementSizes { get; } = ImmutableArray.Create(
        (uint)sizeof(Vector3),
        (uint)sizeof(Vector3),
        (uint)sizeof(Vector4),
        (uint)sizeof(Vector2),
        (uint)sizeof(Vector2),
        (uint)sizeof(Vector3),
        (uint)sizeof(Vector4),
        (uint)sizeof(Vector4));
}

public struct MeshSerializable : ISerializableAsset<Mesh>, IDeserializeFrom<StandardMeshFormat>, ISerializeTo<StandardMeshFormat>
{
    enum LengthType : int
    {
        Total,
        Vertex,
        Position,
        Normal,
        Tangent,
        TexCoord0,
        TexCoord1,
        Colors,
        Joints,
        Weights,
        Indices,
    }

    public Mesh Asset { get; set; }

    public uint ByteSize => this.Asset.SerializedByteSize;

    static readonly int _lengthCount = Enum.GetNames(typeof(LengthType)).Length;

    public unsafe void Deserialize(ReadOnlySpan<byte> source)
    {
        ReadOnlySpan<uint> lengthsSpan = source[0..(_lengthCount * sizeof(uint))].As<byte, uint>();

        Mesh.VertexElements elements = Mesh.VertexElements.None;
        for (int i = 2; i < lengthsSpan.Length - 1; i++)
        {
            int value = 1 << (i - 2);
            if (lengthsSpan[i] != 0)
                elements |= (Mesh.VertexElements)value;
        }

        GraphicsDevice device = Rendering.GraphicsDevice;
        ResourceFactory factory = Rendering.ResourceFactory;

        DeviceBuffer vertexBuffer = factory.CreateBuffer(new(lengthsSpan[(int)LengthType.Vertex], BufferUsage.VertexBuffer));
        DeviceBuffer indexBuffer = factory.CreateBuffer(new(lengthsSpan[(int)LengthType.Indices], BufferUsage.IndexBuffer));

        int vertexOffset = _lengthCount * sizeof(uint);
        uint vertexLength = lengthsSpan[(int)LengthType.Vertex];
        uint indicesLength = lengthsSpan[(int)LengthType.Indices];

        fixed (byte* bytePtr = source)
        {
            device.UpdateBuffer(vertexBuffer, 0, (IntPtr)bytePtr + vertexOffset, vertexLength);

            device.UpdateBuffer(indexBuffer, 0, (IntPtr)bytePtr + vertexOffset + (int)vertexLength, indicesLength);
        }

        this.Asset = new()
        {
            IndexBuffer = indexBuffer,
            VertexBuffer = vertexBuffer,
            IndexCount = indicesLength / sizeof(ushort),
            VertexCount = lengthsSpan[(int)LengthType.Position] / (uint)sizeof(Vector3),
            Elements = elements,
            SerializedByteSize = lengthsSpan[(int)LengthType.Total],
        };
    }

    public unsafe void Deserialize(Stream source)
    {
        Span<byte> lengthsSpan = stackalloc byte[_lengthCount * sizeof(uint)];
        _ = source.Read(lengthsSpan);

        Mesh.VertexElements elements = Mesh.VertexElements.None;
        for (int i = 2; i < lengthsSpan.Length - 1; i++)
        {
            int value = 1 << (i - 2);
            if (lengthsSpan[i] != 0)
                elements |= (Mesh.VertexElements)value;
        }

        GraphicsDevice device = Rendering.GraphicsDevice;
        ResourceFactory factory = Rendering.ResourceFactory;

        DeviceBuffer vertexBuffer = factory.CreateBuffer(new(lengthsSpan[(int)LengthType.Vertex], BufferUsage.VertexBuffer));
        DeviceBuffer indexBuffer = factory.CreateBuffer(new(lengthsSpan[(int)LengthType.Indices], BufferUsage.IndexBuffer));

        int vertexOffset = _lengthCount * sizeof(uint);
        uint vertexLength = lengthsSpan[(int)LengthType.Vertex];
        uint indicesLength = lengthsSpan[(int)LengthType.Indices];

        Span<byte> dataSpan = new byte[source.Length - source.Position];
        _ = source.Read(dataSpan);

        fixed (byte* bytePtr = dataSpan)
        {
            device.UpdateBuffer(vertexBuffer, 0, (IntPtr)bytePtr + vertexOffset, vertexLength);

            device.UpdateBuffer(indexBuffer, 0, (IntPtr)bytePtr + vertexOffset + (int)vertexLength, indicesLength);
        }

        this.Asset = new()
        {
            IndexBuffer = indexBuffer,
            VertexBuffer = vertexBuffer,
            IndexCount = indicesLength / sizeof(ushort),
            VertexCount = lengthsSpan[(int)LengthType.Position] / (uint)sizeof(Vector3),
            Elements = elements,
            SerializedByteSize = lengthsSpan[(int)LengthType.Total],
        };
    }

    public unsafe void Serialize(Span<byte> destination)
    {
        GraphicsDevice device = Rendering.GraphicsDevice;
        ResourceFactory factory = Rendering.ResourceFactory;

        Span<uint> lengthsSpan = stackalloc uint[_lengthCount];

        uint vertexSize = 0;
        for (int i = 1; i < Mesh.ElementCount; i++)
        {
            int enumValue = 1 << (i - 1);
            uint elementSize = ((int)this.Asset.Elements & enumValue) != 0
                ? Mesh.ElementSizes[i - 1] * this.Asset.VertexCount
                : 0;
            lengthsSpan[i + 1] = elementSize;
            vertexSize += elementSize;
        }

        lengthsSpan[(int)LengthType.Vertex] = vertexSize;
        lengthsSpan[(int)LengthType.Indices] = this.Asset.IndexCount * sizeof(ushort);
        lengthsSpan[(int)LengthType.Total] =
            lengthsSpan[(int)LengthType.Vertex] +
            lengthsSpan[(int)LengthType.Indices] +
            ((uint)_lengthCount * sizeof(uint));

        lengthsSpan.As<uint, byte>().CopyTo(destination);

        using DeviceBuffer vertexBuffer = factory.CreateBuffer(
            new(this.Asset.VertexBuffer.SizeInBytes,
                BufferUsage.Staging));

        using DeviceBuffer indexBuffer = factory.CreateBuffer(
            new(this.Asset.IndexBuffer.SizeInBytes,
                BufferUsage.Staging));

        using CommandList commands = factory.CreateCommandList();
        using Fence fence = factory.CreateFence(false);
        commands.Begin();
        commands.CopyBuffer(this.Asset.VertexBuffer, 0, vertexBuffer, 0, this.Asset.VertexBuffer.SizeInBytes);
        commands.CopyBuffer(this.Asset.IndexBuffer, 0, indexBuffer, 0, this.Asset.IndexBuffer.SizeInBytes);
        commands.End();
        device.SubmitCommands(commands, fence);
        device.WaitForFence(fence);

        Span<byte> vertexDestination = destination[(int)lengthsSpan.ByteLength()..];
        Span<byte> indexDestination = vertexDestination[(int)lengthsSpan[(int)LengthType.Vertex]..];

        MappedResourceView<byte> map = device.Map<byte>(vertexBuffer, MapMode.Read);
        ReadOnlySpan<byte> vertexSpan = new((byte*)map.MappedResource.Data, (int)map.SizeInBytes);

        map = device.Map<byte>(indexBuffer, MapMode.Read);
        ReadOnlySpan<byte> indexSpan = new((byte*)map.MappedResource.Data, (int)map.SizeInBytes);

        vertexSpan.CopyTo(vertexDestination);
        indexSpan.CopyTo(indexDestination);
    }

    public unsafe void Serialize(Stream destination)
    {
        GraphicsDevice device = Rendering.GraphicsDevice;
        ResourceFactory factory = Rendering.ResourceFactory;

        Span<uint> lengthsSpan = stackalloc uint[_lengthCount];

        uint vertexSize = 0;
        for (int i = 1; i < Mesh.ElementCount; i++)
        {
            int enumValue = 1 << (i - 1);
            uint elementSize = ((int)this.Asset.Elements & enumValue) != 0
                ? Mesh.ElementSizes[i - 1] * this.Asset.VertexCount
                : 0;
            lengthsSpan[i + 1] = elementSize;
            vertexSize += elementSize;
        }

        lengthsSpan[(int)LengthType.Vertex] = vertexSize;
        lengthsSpan[(int)LengthType.Indices] = this.Asset.IndexCount * sizeof(ushort);
        lengthsSpan[(int)LengthType.Total] =
            lengthsSpan[(int)LengthType.Vertex] +
            lengthsSpan[(int)LengthType.Indices] +
            ((uint)_lengthCount * sizeof(uint));

        destination.Write(lengthsSpan.As<uint, byte>());

        using DeviceBuffer vertexBuffer = factory.CreateBuffer(
            new(this.Asset.VertexBuffer.SizeInBytes,
                BufferUsage.Staging));

        using DeviceBuffer indexBuffer = factory.CreateBuffer(
            new(this.Asset.IndexBuffer.SizeInBytes,
                BufferUsage.Staging));

        using CommandList commands = factory.CreateCommandList();
        using Fence fence = factory.CreateFence(false);
        commands.Begin();
        commands.CopyBuffer(this.Asset.VertexBuffer, 0, vertexBuffer, 0, this.Asset.VertexBuffer.SizeInBytes);
        commands.CopyBuffer(this.Asset.IndexBuffer, 0, indexBuffer, 0, this.Asset.IndexBuffer.SizeInBytes);
        commands.End();
        device.SubmitCommands(commands, fence);
        device.WaitForFence(fence);

        MappedResourceView<byte> map = device.Map<byte>(vertexBuffer, MapMode.Read);
        ReadOnlySpan<byte> vertexSpan = new((byte*)map.MappedResource.Data, (int)map.SizeInBytes);

        map = device.Map<byte>(indexBuffer, MapMode.Read);
        ReadOnlySpan<byte> indexSpan = new((byte*)map.MappedResource.Data, (int)map.SizeInBytes);

        destination.Write(vertexSpan);
        destination.Write(indexSpan);
    }
}
