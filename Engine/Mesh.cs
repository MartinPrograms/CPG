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
            // Loading in mesh data isn't too different from OpenGL
            // First we create a GpuBuffer object, which contains the type, size in bytes, count and the usage/access settings
            // And it contains a pointer to the data
            var vertexBuffer = new GpuBuffer<float>(pVertices, (uint)(Vertices.Length * sizeof(float))
                , Vertices.Length,new GpuBufferSettings(BufferUsage.Static, BufferAccess.ReadOnly));

            // Then we create a VertexBufferSettings object, which contains the GpuBuffer object, the stride, and the attributes
            var vertexBufferSettings = new VertexBufferSettings<float>(vertexBuffer, 5 * sizeof(float), // 5 floats per vertex (3 for position, 2 for texture coordinates)
                
            [
                // Position attribute
                new VertexAttribute(0, 3, 0, VertexAttributeType.Float,
                        false), // Index 0, 3 elements, 0 offset, float type, not normalized
                // UV/Texture attribute
                new VertexAttribute(1, 2, 3 * sizeof(float), VertexAttributeType.Float,
                        false) // Index 1, 2 elements, 3 * sizeof(float) offset, float type, not normalizeds
            ]);

            // Then we send it to the graphics API to create the vertex buffer
            VertexBuffer = _GraphicsApi.CreateVertexBuffer(vertexBufferSettings);
            
            // This approach is more verbose than OpenGL, however it allows for more control, because it's not a giant state machine which can be hard to debug
            // So now (just like vulkan) you get settings objects, and you pass them to the graphics API to create resources.
            // In my opinion a better approach than OpenGL's state machine.
        }

        fixed (uint* pIndices = &Indices[0])
        {
            // Again like before, pass in a pointer to the data with some settings
            var indexBuffer = new GpuBuffer<uint>(pIndices, (uint)(Indices.Length * sizeof(uint)),
                Indices.Length,new GpuBufferSettings(BufferUsage.Static, BufferAccess.ReadOnly));
            
            var indexBufferSettings = new IndexBufferSettings<uint>(indexBuffer, Indices.Length);
            
            // And tell the graphics API to create the index buffer
            IndexBuffer = _GraphicsApi.CreateIndexBuffer(indexBufferSettings);
        }
        
        // Unused in OpenGL, might be useful in other APIs
        VertexBuffer.LinkIndexBuffer<uint>(IndexBuffer);
        
        _GraphicsApi.UnbindVertexBuffer();
        _GraphicsApi.UnbindIndexBuffer();
    }
    
    public void Draw()
    {
        // We still have some remnants of OpenGLs paradigm, like the use of the shader object
        // But personally I do not mind this, because it's not too bad, and the difference here is we do have to call SetUniform on the shader object instead of gl.GetUniformLocation and gl.Uniform
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
        
        // Specify the vertex and index buffers to use
        // We need to specify the type to use, because the graphics API is generic
        var settings = new DrawCallSettings<float, uint>(VertexBuffer, IndexBuffer, Primitive.Triangle, Vertices.Length, Indices.Length, 0);
        _GraphicsApi.Draw(settings);
    }
}