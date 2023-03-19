namespace Xiphos.Resources;

public interface ISerializationFormat
{ 
    static abstract string AssetCode { get; }
}

public interface ISerializableAsset<T> 
{
    public T Asset { get; set; }
}

public interface ISerializeTo<TFormat>
    where TFormat : ISerializationFormat
{
    uint ByteSize { get; }

    void Serialize(Span<byte> destination);

    void Serialize(Stream stream);
}

public interface IDeserializeFrom<TFormat>
    where TFormat : ISerializationFormat
{
    void Deserialize(ReadOnlySpan<byte> source);

    void Deserialize(Stream stream);
}

public static class SerializableExtensions
{
    public static void SerializeAs<T, TFormat>(this T serializable, Span<byte> destination)
        where T : ISerializeTo<TFormat>
        where TFormat : ISerializationFormat
        => serializable.Serialize(destination);

    public static void DeserializeAs<T, TFormat>(this T serializable, Span<byte> source)
        where T : IDeserializeFrom<TFormat>
        where TFormat : ISerializationFormat
        => serializable.Deserialize(source);

    public static void SerializeAs<T, TFormat>(this T serializable, FileInfo destination)
        where T : ISerializeTo<TFormat>
        where TFormat : ISerializationFormat
    {
        using FileStream stream = destination.Open(FileMode.OpenOrCreate, FileAccess.Write);

        Span<byte> buffer = new byte[serializable.ByteSize];

        serializable.Serialize(buffer);

        stream.Write(buffer);
    }

    public static void DeserializeAs<T, TFormat>(this T serializable, FileInfo source)
        where T : IDeserializeFrom<TFormat>
        where TFormat : ISerializationFormat
    {
        using FileStream stream = source.Open(FileMode.Open, FileAccess.Read);

        Span<byte> buffer = new byte[stream.Length];

        _ = stream.Read(buffer);

        serializable.Deserialize(buffer);
    }
}