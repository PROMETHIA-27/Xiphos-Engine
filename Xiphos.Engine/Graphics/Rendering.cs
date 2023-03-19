using static Xiphos.ECS3.EntityQuery;

namespace Xiphos.Graphics;
public static class Rendering
{
    private static GraphicsDevice _graphicsDevice;
    private static CommandList _commandList;
    private static DeviceBuffer _vertexBuffer;
    private static DeviceBuffer _indexBuffer;
    private static DeviceBuffer _instanceBuffer;
    private static DeviceBuffer _projViewBuffer;
    private static Shader[] _shaders;
    private static Pipeline _pipeline;
    private static ResourceSet _resources;

    internal static GraphicsDevice GraphicsDevice { get; private set; }
    internal static ResourceFactory ResourceFactory { get; private set; }

    public const string vertexShaderSource =
@"#version 450

layout(set = 0, binding = 0) uniform ProjViewBuffer
{
    mat4 Projection;
    mat4 View;
};

layout(location = 0) in vec3 Position;
layout(location = 1) in vec3 Normal;
layout(location = 2) in vec4 Row1;
layout(location = 3) in vec4 Row2;
layout(location = 4) in vec4 Row3;
layout(location = 5) in vec4 Row4;

layout(location = 0) out vec4 fsin_Color;
layout(location = 1) out vec4 fsin_WorldPosition;

void main()
{
    mat4 world = mat4(Row1, Row2, Row3, Row4);
    vec4 worldPosition = world * vec4(Position, 1);
    fsin_WorldPosition = worldPosition;
    vec4 viewPosition = View * worldPosition;
    vec4 clipPosition = Projection * viewPosition;
    gl_Position = clipPosition;
    fsin_Color = vec4(abs(normalize(Normal)), 1);
}";

    public const string fragmentShaderSource =
@"#version 450

layout(location = 0) in vec4 fsin_Color;
layout(location = 1) in vec4 fsin_WorldPosition;

layout(location = 0) out vec4 fsout_Color;

void main()
{
    //fsout_Color = vec4(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z, 1.0);
    fsout_Color = fsin_Color;
}";

    public static unsafe void OpenWindow()
    {
        WindowCreateInfo windowCI = new()
        {
            X = 100,
            Y = 100,
            WindowWidth = 1920,
            WindowHeight = 1080,
            WindowTitle = "Veldrid Tutorial",
        };
        Sdl2Window window = VeldridStartup.CreateWindow(ref windowCI);

        GraphicsDeviceOptions options = new()
        {
            PreferStandardClipSpaceYDirection = true,
            PreferDepthRangeZeroToOne = true,
            SwapchainDepthFormat = PixelFormat.R32_Float
        };
        _graphicsDevice = VeldridStartup.CreateGraphicsDevice(window, options);
        _graphicsDevice.SyncToVerticalBlank = true;

        GraphicsDevice = _graphicsDevice;
        ResourceFactory = _graphicsDevice.ResourceFactory;

        CreateResources();

        while (window.Exists)
        {
            InputSnapshot input = window.PumpEvents();

            if (input.IsMouseDown(MouseButton.Left))
                //Update entity transforms
                _time += 0.0166d;

            Matrix4x4 rotation =
                Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, (float)_time) *
                Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, (float)_time / 3.0f);

            EntityState[] states = World<Main>
                .Query(Has<TransformParent>())
                .OrderBy(state => state.GetFilterValue<TransformParent, int>().Some)
                .ToArray();

            for (int i = 0; i < states.Length; i++)
            {
                ReadOnlyView<TransformParent> parentView = states[i].GetReadOnlyBuffer<TransformParent>().RightProjection;
                View<Transform> transformView = states[i].GetBuffer<Transform>().RightProjection;

                if (states[i].GetFilterValue<TransformParent, int>().Some > 0)
                    _ = Parallel.For(0, (int)states[i].Count, i =>
                    {
                        ref readonly TransformParent parent = ref parentView[i];
                        ref Transform trans = ref transformView[i];

                        trans._matrix = rotation * parent.LocalMatrix * parent.GetParentTransform().Some.WorldMatrix;
                    });
                else
                    _ = Parallel.For(0, (int)states[i].Count, i =>
                    {
                        ref readonly TransformParent parent = ref parentView[i];
                        ref Transform trans = ref transformView[i];

                        trans._matrix = rotation * parent.LocalMatrix;
                    });
            }

            RenderCubeFromEntity();
        }

        DisposeResources();
    }

    private static double _time = 0.0d;
    public static unsafe void RenderCubeFromEntity()
    {
        EntityState[] states = World<Main>.Query(And(Has<Transform>(), Has<RenderMeshInstanced>()));

        _commandList.Begin();
        _commandList.SetFramebuffer(_graphicsDevice.SwapchainFramebuffer);
        _commandList.ClearColorTarget(0, RgbaFloat.Black);
        _commandList.ClearDepthStencil(1.0f);
        _commandList.SetPipeline(_pipeline);
        _commandList.UpdateBuffer(_projViewBuffer, 0, Matrix4x4.CreatePerspectiveFieldOfView(1.0f, 16.0f / 9.0f, 0.5f, 100.0f));
        _commandList.UpdateBuffer(_projViewBuffer, (uint)sizeof(Matrix4x4), Matrix4x4.CreateLookAt(Vector3.UnitZ * 2.5f, Vector3.Zero, Vector3.UnitY));
        _commandList.SetVertexBuffer(0, _vertexBuffer);
        _commandList.SetVertexBuffer(1, _vertexBuffer, 24 * (uint)sizeof(Vector3));
        _commandList.SetVertexBuffer(2, _instanceBuffer);
        _commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
        _commandList.SetGraphicsResourceSet(0, _resources);

        for (int i = 0; i < states.Length; i++)
        {
            ref EntityState state = ref states[i];
            ReadOnlyView<Transform> transforms = state.GetReadOnlyBuffer<Transform>().RightProjection;
            ReadOnlyView<RenderMeshInstanced> renderMeshes = state.GetReadOnlyBuffer<RenderMeshInstanced>().RightProjection;

            _commandList.UpdateBuffer(_instanceBuffer, 0, (IntPtr)transforms._ptr, (uint)transforms.Length * (uint)sizeof(Transform));
            _commandList.DrawIndexed(
                indexCount: 36,
                instanceCount: (uint)transforms.Length,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);
        }

        _commandList.End();
        _graphicsDevice.SubmitCommands(_commandList);
        _graphicsDevice.SwapBuffers();
        _graphicsDevice.WaitForIdle();
    }

    private static readonly Gltf _gltf = glTFLoader.Interface.LoadModel("C:/Users/elect/Desktop/Cube.gltf");
    private static unsafe void CreateResources()
    {
        ResourceFactory factory = _graphicsDevice.ResourceFactory;

        byte[] binary = File.ReadAllBytes("C:/Users/elect/Desktop/Cube.bin");
        byte[][]? binaries = new byte[][] { binary };



        //MeshPrimitive primitive = _gltf.GetMesh(binaries, 0).Primitives[0];
        MeshPrimitive primitive = default;

        //ReadOnlySpan<Vector3> vertSpan = primitive.Positions!.Value.As<Vector3>();
        //ReadOnlySpan<Vector3> normSpan = primitive.Normals!.Value.As<Vector3>();
        //ReadOnlySpan<ushort> idxSpan = primitive.Indices.As<ushort>();

        ReadOnlySpan<Vector3> vertSpan, normSpan;
        ReadOnlySpan<ushort> idxSpan;
        vertSpan = default;
        normSpan = default;
        idxSpan = default;

        _vertexBuffer = factory.CreateBuffer(new(vertSpan.ByteLength() + normSpan.ByteLength(), BufferUsage.VertexBuffer));
        _indexBuffer = factory.CreateBuffer(new(idxSpan.ByteLength(), BufferUsage.IndexBuffer));

        _graphicsDevice.UpdateBuffer(_vertexBuffer, 0, ref MemoryMarshal.GetReference(vertSpan), vertSpan.ByteLength());
        _graphicsDevice.UpdateBuffer(_vertexBuffer, vertSpan.ByteLength(), ref MemoryMarshal.GetReference(normSpan), normSpan.ByteLength());
        _graphicsDevice.UpdateBuffer(_indexBuffer, 0, ref MemoryMarshal.GetReference(idxSpan), idxSpan.ByteLength());

        _projViewBuffer = factory.CreateBuffer(new BufferDescription((uint)sizeof(Matrix4x4) * 2, BufferUsage.UniformBuffer));

        _instanceBuffer = factory.CreateBuffer(new((uint)sizeof(Matrix4x4) * 7, BufferUsage.VertexBuffer));

        ResourceLayout projViewLayout = factory.CreateResourceLayout(
            new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("ProjViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex))
            );

        VertexLayoutDescription vertexLayout = new(
            new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3));
        VertexLayoutDescription normalLayout = new(
            new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3));

        VertexLayoutDescription instanceLayout = new(
            new VertexElementDescription("Row1", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4),
            new VertexElementDescription("Row2", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4),
            new VertexElementDescription("Row3", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4),
            new VertexElementDescription("Row4", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4)
            );
        instanceLayout.InstanceStepRate = 1;

        ShaderDescription vertexShaderDesc = new(
            ShaderStages.Vertex,
            Encoding.UTF8.GetBytes(vertexShaderSource),
            "main");
        ShaderDescription fragmentShaderDesc = new(
            ShaderStages.Fragment,
            Encoding.UTF8.GetBytes(fragmentShaderSource),
            "main");

        _shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

        GraphicsPipelineDescription pipelineDescription = new()
        {
            BlendState = BlendStateDescription.SingleOverrideBlend,
            DepthStencilState = new()
            {
                DepthTestEnabled = true,
                DepthWriteEnabled = true,
                DepthComparison = ComparisonKind.LessEqual,
            },
            RasterizerState = new()
            {
                CullMode = FaceCullMode.Back,
                FillMode = PolygonFillMode.Solid,
                FrontFace = FrontFace.CounterClockwise,
                DepthClipEnabled = true,
                ScissorTestEnabled = false,
            },
            PrimitiveTopology = PrimitiveTopology.TriangleList,
            ResourceLayouts = new[] { projViewLayout },
            ShaderSet = new(new[] { vertexLayout, normalLayout, instanceLayout }, _shaders),
            Outputs = _graphicsDevice.SwapchainFramebuffer.OutputDescription,
        };

        _resources = factory.CreateResourceSet(new(
                projViewLayout,
                _projViewBuffer
            ));

        _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
        _commandList = factory.CreateCommandList();
    }

    private static void DisposeResources()
    {
        _pipeline.Dispose();
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _shaders[0].Dispose();
        _shaders[1].Dispose();
        _commandList.Dispose();
        _graphicsDevice.Dispose();
    }
}