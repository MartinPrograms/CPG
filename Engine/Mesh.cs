using CPG.Common.Rendering;
using CPG.Interface;

namespace Engine;

public class Mesh
{
    public Transform Transform { get; set; } = Transform.Default();
    
    private Shader _shader;
    private IGraphicsApi _GraphicsApi;
    public Mesh(IGraphicsApi objGraphicsApi, Shader shader)
    {
        _GraphicsApi = objGraphicsApi;
        _shader = shader;
    }
    
    public float[] Vertices { get; set; }
    public uint[] Indices { get; set; }
    public IVertexBuffer VertexBuffer { get; set; }
    public IIndexBuffer IndexBuffer { get; set; }

    public unsafe void Init()
    {
        fixed (float* pVertices = &Vertices[0])
        {
            var vertexBuffer = new GpuBuffer<float>(pVertices, (uint)(Vertices.Length * sizeof(float))
                , Vertices.Length,new GpuBufferSettings(BufferUsage.Static, BufferAccess.ReadOnly));

            var vertexBufferSettings = new VertexBufferSettings<float>(vertexBuffer, 3 * sizeof(float),
            [
                new VertexAttribute(0, 3, 0, VertexAttributeType.Float,
                        false) // Index 0, 3 elements, 0 offset, float type, not normalized
            ]);

            VertexBuffer = _GraphicsApi.CreateVertexBuffer(vertexBufferSettings);
        }

        fixed (uint* pIndices = &Indices[0])
        {
            var indexBuffer = new GpuBuffer<uint>(pIndices, (uint)(Indices.Length * sizeof(uint)),
                Indices.Length,new GpuBufferSettings(BufferUsage.Static, BufferAccess.ReadOnly));
            
            var indexBufferSettings = new IndexBufferSettings<uint>(indexBuffer, Indices.Length);
            IndexBuffer = _GraphicsApi.CreateIndexBuffer(indexBufferSettings);
        }
        
        VertexBuffer.LinkIndexBuffer<uint>(IndexBuffer);
        
        _GraphicsApi.UnbindVertexBuffer();
        _GraphicsApi.UnbindIndexBuffer();
    }
    
    public void Draw()
    {
        _shader.Use();
        var settings = new DrawCallSettings<float, uint>(VertexBuffer, IndexBuffer, Primitive.Triangle, Vertices.Length, Indices.Length, 0);
        _GraphicsApi.Draw(settings);
    }
}