using System.Numerics;
using CPG.Common.Rendering;
using CPG.Interface;

namespace Engine;

public class Mesh
{
    public Transform Transform { get; set; } = Transform.Default();
    
    public List<Texture> Textures { get; set; } = new List<Texture>();
    
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

            var vertexBufferSettings = new VertexBufferSettings<float>(vertexBuffer, 5 * sizeof(float), // 5 floats per vertex (3 for position, 2 for texture coordinates)
                
            [
                new VertexAttribute(0, 3, 0, VertexAttributeType.Float,
                        false), // Index 0, 3 elements, 0 offset, float type, not normalized
                new VertexAttribute(1, 2, 3 * sizeof(float), VertexAttributeType.Float,
                        false) // Index 1, 2 elements, 3 * sizeof(float) offset, float type, not normalizeds
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

        foreach (var texture in Textures)
        {
            var textureIndex = (int)Textures.IndexOf(texture);
            texture.Use((uint)textureIndex);
            
            if (!_shader.SetUniform($"texture{textureIndex}", textureIndex))
            {
                Console.WriteLine($"Failed to set uniform texture{textureIndex}");
            }
        }
        
        if (!_shader.SetUniform("model", Transform.GetModelMatrix()))
        {
            Console.WriteLine("Failed to set uniform model");
        }
        
        var settings = new DrawCallSettings<float, uint>(VertexBuffer, IndexBuffer, Primitive.Triangle, Vertices.Length, Indices.Length, 0);
        _GraphicsApi.Draw(settings);
    }
}