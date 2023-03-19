using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using Pidgin;

namespace Xiphos;

internal class Program
{
    readonly struct URIRep
    {
        public string? Scheme { get; init; }
        public string? MediaType { get; init; }
        public string[] Parameters { get; init; }
        public string[] Values { get; init; }
        public bool IsBase64 { get; init; }
        public string? Authority { get; init; } 
        public string Path { get; init; }
        public string? Query { get; init; }
        public string? Fragment { get; init; }
    }

    static Parser<char, T> Tok<T>(Parser<char, T> p)
        => Try(p);
    
    static Parser<char, char> Tok(char value) => Tok(Char(value));
    static Parser<char, string> Tok(string value) => Tok(String(value));

    static readonly Parser<char, char> _comma = Tok(',');
    static readonly Parser<char, char> _semicolon = Tok(';');
    static readonly Parser<char, char> _colon = Tok(':');
    static readonly Parser<char, char> _equals = Tok('=');
    static readonly Parser<char, char> _forwardSlash = Tok('/');
    static readonly Parser<char, string> _doubleForwardSlash = Tok("//");
    static readonly Parser<char, char> _questionMark = Tok('?');
    static readonly Parser<char, char> _poundSign = Tok('#');

    static readonly Parser<char, string> _media = AnyCharExcept(';', ',').AtLeastOnceString();
    static readonly Parser<char, string> _parameter = AnyCharExcept('=').AtLeastOnceString();
    static readonly Parser<char, string> _value = AnyCharExcept(';', ',').AtLeastOnceString();

    static readonly Parser<char, URIRep> _parameters = 
        Try(
            _semicolon
            .Then(_parameter)
            .Before(_equals)
            .Then(_value, (p, v) => (p, v))
            )
        .Many()
        .Map(r => r.ToArray().Flatten())
        .Map(r => new URIRep() 
        { 
            Parameters = r.Item1, 
            Values = r.Item2 
        });

    static readonly Parser<char, URIRep> _mediaType =
        _media
        .Then(_parameters, (m, u) => u with { MediaType = m })
        .Then(Try(String(";base64")).Optional(), (u, b) => u with { IsBase64 = b.HasValue })
        .Before(_comma);

    static readonly Parser<char, string> _scheme = AnyCharExcept(':').ManyString().Before(_colon);

    static readonly Parser<char, URIRep> _preAuth = 
        Try(
            _scheme
            .Then(Try(_mediaType).Optional(), (s, u) => u.GetValueOrDefault() with { Scheme = s })
            )
        .Optional()
        .Map(t => t.GetValueOrDefault());

    static readonly Parser<char, string> _authority = AnyCharExcept('/').AtLeastOnceString();

    static readonly Parser<char, string?> _auth = Try(_doubleForwardSlash.Then(_authority).Before(_forwardSlash)).Optional().Map(a => a.HasValue ? a.Value : null);
    static readonly Parser<char, string> _path = AnyCharExcept('?').AtLeastOnceString();
    static readonly Parser<char, string?> _query = Try(_questionMark.Then(AnyCharExcept('#').AtLeastOnceString())).Optional().Map(q => q.HasValue ? q.Value : null);
    static readonly Parser<char, string?> _fragment = Try(_poundSign.Then(Any.AtLeastOnceString())).Optional().Map(f => f.HasValue ? f.Value : null);

    static readonly Parser<char, URIRep> _uri = 
        _preAuth
        .Then(_auth, (u, a) => u with { Authority = a })
        .Then(_path, (u, p) => u with { Path = p })
        .Then(_query, (u, q) => u with { Query = q })
        .Then(_fragment, (u, f) => u with { Fragment = f });

    public static void Main()
    {
        string bufferUri = "data:";

        SpanStream<char> uriStr = new(bufferUri);
        
        // media type = {type}<;{parameter}={value}>[;base64],
        // [{scheme}:[media type]][//{Authority}/]{Path}[?{Query}][#{Fragment}]

        //var parameter = _parameters2.ParseOrThrow(";Encoding=UTF8;Fuckery=12;Bruh=Moment");

        var @base = _mediaType.ParseOrThrow("media/image;Encoding=UTF8,");

        var uri = _uri.ParseOrThrow(bufferUri);

        StringBuilder type = new();
        //StringBuilder parameter = new();
        StringBuilder value = new();

        //URI uri = new(bufferUri);

        //StringBuilder scheme = new();
        //StringBuilder authority = new();
        //StringBuilder path = new();
        //StringBuilder query = new();
        //StringBuilder fragment = new();

        return;

        byte[] meshData = new byte[212];

        using MemoryStream stream = new(meshData, true);

        ReadOnlySpan<uint> values = stackalloc uint[] { 44 + ((12 * 12) + 24), (12 * 12) + (24), 12 * 12, 0, 0, 0, 0, 0, 0, 0, 2 * 12 };
        stream.Write(values.As<uint, byte>());
        Vector3 position = new(69, 69, 69);
        stream.Write(position.GetBytes());

        MeshSerializable ser = new();
        ser.Deserialize(meshData);

        ser.Serialize(meshData);

        EntityState rootState = new(
            new EntityStateDescription(1024)
            .AddComponent<Transform>()
            .AddComponent<TransformParent, int>(0)
            .AddComponent<RenderMeshInstanced>());

        EntityState childState = new(
            new EntityStateDescription(1024)
            .AddComponent<Transform>()
            .AddComponent<TransformParent, int>(1)
            .AddComponent<RenderMeshInstanced>());

        EntityIndex entity = rootState
            .CreateEntityWith(new Transform(Matrix4x4.CreateTranslation(new(0, 0, -3))), new TransformParent(Matrix4x4.CreateTranslation(new(-3, 0, -3))))
            .RightProjection
            .Index;

        Matrix4x4 up = Matrix4x4.Identity;
        Matrix4x4 down = Matrix4x4.Identity;
        Matrix4x4 left = Matrix4x4.Identity;
        Matrix4x4 right = Matrix4x4.Identity;
        Matrix4x4 forward = Matrix4x4.Identity;
        Matrix4x4 backward = Matrix4x4.Identity;

        up.Translation += new Vector3(0, 1, 0);
        down.Translation += new Vector3(0, -1, 0);
        left.Translation += new Vector3(-1, 0, 0);
        right.Translation += new Vector3(1, 0, 0);
        forward.Translation += new Vector3(0, 0, 1);
        backward.Translation += new Vector3(0, 0, -1);

        QualifiedEntityIndex qualEntity = new(entity, rootState);

        _ = childState.CreateEntityWith(new TransformParent(up, qualEntity));
        _ = childState.CreateEntityWith(new TransformParent(down, qualEntity));
        _ = childState.CreateEntityWith(new TransformParent(left, qualEntity));
        _ = childState.CreateEntityWith(new TransformParent(right, qualEntity));
        _ = childState.CreateEntityWith(new TransformParent(forward, qualEntity));
        _ = childState.CreateEntityWith(new TransformParent(backward, qualEntity));

        _ = rootState.HandleEvent(new MoveUp() { amount = 1 });

        _ = rootState.HandleEvent(new MoveUp() { amount = -1 });

        _ = rootState.SendMessage(new MoveLeft(), entity);

        World<Main>.AddState(rootState);

        World<Main>.AddState(childState);

        Rendering.OpenWindow();
    }
}

internal struct MoveUp : IComponentEvent
{
    public int amount;
}

internal record struct MoveLeft(EntityIndex Index) : IComponentMessage;

internal struct Transform : 
    IComponent, 
    IInitializer<Transform>, 
    IEventResponder<MoveUp, Transform>,
    IMessageResponder<MoveLeft, Transform>
{
    internal Matrix4x4 _matrix;
    public Matrix4x4 WorldMatrix => this._matrix;

    public Transform()
        => this._matrix = Matrix4x4.Identity;

    public Transform(Matrix4x4 matrix)
        => this._matrix = matrix;

    public static void Initialize(out Transform @this)
        => @this._matrix = Matrix4x4.Identity;

    public static void Respond(in MoveUp e, ref Transform trans)
        => trans._matrix.Translation += new Vector3(0, e.amount, 0);

    public static void Respond(in MoveLeft e, ref Transform trans)
        => trans._matrix.Translation += new Vector3(-3, 0, 0);
}

internal struct TransformParent :
    IComponent,
    IFilter<int, TransformParent>
{
    Matrix4x4 _matrix;
    QualifiedEntityIndex _parent;

    public Matrix4x4 LocalMatrix => this._matrix;
    public bool HasParent => this._parent.StateDataPtr != IntPtr.Zero;
    public int Depth => this.HasParent ? this._parent.GetComponent<TransformParent>().RightProjection.Depth + 1 : 0;

    public TransformParent(Matrix4x4 matrix)
        => (this._matrix, this._parent) = (matrix, default);

    public TransformParent(Matrix4x4 matrix, QualifiedEntityIndex parentIndex)
        => (this._matrix, this._parent) = (matrix, parentIndex);

    public Xiphos.Monads.Maybe<Transform> GetParentTransform()
        => this.HasParent ? this._parent.GetComponent<Transform>().RightMaybe : new();

    public static bool CheckComponent(in int stateValue, in TransformParent self)
        => stateValue == self.Depth;

    public static bool CheckQuery(in int stateValue, in int comparisonValue)
        => stateValue == comparisonValue;

    public static void Respond(in MoveUp e, ref TransformParent trans)
        => trans._matrix.Translation += new Vector3(0, e.amount, 0);

    public static void Respond(in MoveLeft e, ref TransformParent trans)
        => trans._matrix.Translation += new Vector3(-3, 0, 0);
}

internal readonly struct RenderMeshInstanced : IComponent, IEquatable<RenderMeshInstanced>
{
    public RID MeshRID { get; init; }

    public bool Equals(RenderMeshInstanced other)
        => this.MeshRID == other.MeshRID;

    public override bool Equals(object? obj)
        => obj is RenderMeshInstanced other && this.Equals(other);

    public override int GetHashCode()
        => this.MeshRID.GetHashCode();
}

//public readonly struct Mesh
//{
//    internal byte[][] Binaries { get; init; }
//    internal MeshPrimitive[] Primitives { get; init; }
//}

//internal unsafe readonly struct MeshPrimitive
//{
//    internal ReadOnlyMemory<byte>? Positions { get; init; }         //vec3
//    internal ReadOnlyMemory<byte>? Normals { get; init; }           //vec3
//    internal ReadOnlyMemory<byte>? Tangents { get; init; }          //vec4
//    internal ReadOnlyMemory<byte>? TexCoords0 { get; init; }        //vec2
//    internal ReadOnlyMemory<byte>? TexCoords1 { get; init; }        //vec2
//    internal ReadOnlyMemory<byte>? Color { get; init; }             //vec3
//    internal ReadOnlyMemory<byte>? Joints { get; init; }            //vec4
//    internal ReadOnlyMemory<byte>? Weights { get; init; }           //vec4
//    internal ReadOnlyMemory<byte> Indices { get; init; }            //uint
//    internal DeviceBuffer VertexBuffer { get; init; }
//    internal DeviceBuffer IndexBuffer { get; init; }
//    internal Shader[] Shaders { get; init; }
//    internal uint PositionCount => (uint)(this.Positions!.Value.Length / sizeof(Vector3));

//    public MeshPrimitive(
//        ReadOnlyMemory<byte>? positions,
//        ReadOnlyMemory<byte>? normals,
//        ReadOnlyMemory<byte>? tangents,
//        ReadOnlyMemory<byte>? texcoords0,
//        ReadOnlyMemory<byte>? texcoords1,
//        ReadOnlyMemory<byte>? color,
//        ReadOnlyMemory<byte>? joints,
//        ReadOnlyMemory<byte>? weights,
//        ReadOnlyMemory<byte> indices)
//    {
//        this.Positions = positions;
//        this.Normals = normals;
//        this.Tangents = tangents;
//        this.TexCoords0 = texcoords0;
//        this.TexCoords1 = texcoords1;
//        this.Color = color;
//        this.Joints = joints;
//        this.Weights = weights;
//        this.Indices = indices;

//        this.VertexBuffer = Rendering.Factory.CreateBuffer(
//            new((uint)this.Positions!.Value.Length + (uint)this.Normals!.Value.Length, BufferUsage.VertexBuffer));
//        this.IndexBuffer = Rendering.Factory.CreateBuffer(
//            new((uint)this.Indices.Length, BufferUsage.IndexBuffer));

//        ShaderDescription vertexShaderDesc = new(
//            ShaderStages.Vertex,
//            Encoding.UTF8.GetBytes(Rendering.vertexShaderSource),
//            "main");
//        ShaderDescription fragmentShaderDesc = new(
//            ShaderStages.Fragment,
//            Encoding.UTF8.GetBytes(Rendering.fragmentShaderSource),
//            "main");
//        this.Shaders = Rendering.Factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);
//    }

//    public bool TryGetPositions(out ReadOnlySpan<Vector3> positions)
//    {
//        positions = this.Positions is ReadOnlyMemory<byte> mem ? mem.As<Vector3>() : default;
//        return true;
//    }

//    public bool TryGetNormals(out ReadOnlySpan<Vector3> normals)
//    {
//        normals = this.Normals is ReadOnlyMemory<byte> mem ? mem.As<Vector3>() : default;
//        return true;
//    }

//    public ReadOnlySpan<uint> GetIndices()
//        => this.Indices.As<uint>();
//}
