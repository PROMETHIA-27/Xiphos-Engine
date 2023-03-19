//namespace Xiphos.Utilities;

//#pragma warning disable IDE1006 // Naming Styles
//public struct glTFSerializer : IResourceSerializer<Gltf, DefaultResourceError>
//{
//    public static DefaultResourceError Deserialize(byte[] bytes, out Gltf data)
//    {
//        using MemoryStream stream = new(bytes);

//        data = glTFLoader.Interface.LoadModel(stream);

//        return DefaultResourceError.None;
//    }

//    public static ReadOnlyMemory<byte> Serialize(Gltf data)
//    {
//        using MemoryStream stream = new(1024);
//        glTFLoader.Interface.SaveModel(data, stream);
//        return stream.ToArray();
//    }
//}
//#pragma warning restore IDE1006 // Naming Styles
