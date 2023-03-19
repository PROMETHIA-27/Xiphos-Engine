namespace Xiphos.Graphics;

public struct GltfFormat : ISerializationFormat
{
    public static string AssetCode => "GLTF";
}

//public struct GltfSerializable : ISerializableAsset<MeshSerializable[]>, IDeserializeFrom<GltfFormat> //TODO: Serialize to GLTF
//{
//    public MeshSerializable[] Asset { get; set; }

//    public unsafe void Deserialize(ReadOnlySpan<byte> source)
//    {
//        Gltf model;
//        fixed (byte* sourcePtr = source)
//        {
//            using UnmanagedMemoryStream stream = new(sourcePtr, source.Length);

//            model = glTFLoader.Interface.LoadModel(stream);
//        }

//        byte[][] binaries = new byte[model.Buffers.Length][];

//        for (int i = 0; i < model.Buffers.Length; i++)
//        {
//            glTFLoader.Schema.Buffer buffer = model.Buffers[i];

//            Uri bufferUri = new(buffer.Uri);

//            if (bufferUri.Scheme == "data")
//            {

//            }

//            else if (bufferUri.IsFile)
//            {

//            }

//            else
//            {

//            }
//        }

//        for (int i = 0; i < model.Meshes.Length; i++)
//        {
//            glTFLoader.Schema.Mesh mesh = model.Meshes[i];

//            for (int j = 0; j < mesh.Primitives.Length; j++)
//            {
//                MeshPrimitive primitive = mesh.Primitives[j];


//            }
//        }
//    }
//}