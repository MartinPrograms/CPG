using System.Numerics;
using CPG.Common;
using CPG.Common.Rendering;

namespace CPG.Interface;

/// <summary>
/// This is the class that actually calls graphics functions.
/// It is decently abstracted, so it can be used for any graphics API.
/// It follows a vulkan-esque design, where settings are passed in using structs, or records.
/// </summary>
public interface IGraphicsApi
{
    public void Init();
    public void Shutdown();
    
    public void Clear(ClearMask mask);
    public void SetClearColor(Vector4 color);
    
    public void UseShader(IShader shader);
    public bool SetUniform<T>(IShader shader, string name, T value);
    public void UseTexture(ITexture texture, uint slot);
    public void UseVertexBuffer<T>(IVertexBuffer buffer) where T : unmanaged;
    public void UseIndexBuffer<T>(IIndexBuffer buffer)    where T : unmanaged;
    public void UseFrameBuffer(IFrameBuffer buffer);
    
    public void Draw<V,I>(DrawCallSettings<V,I> settings) where V : unmanaged where I : unmanaged;
    
    public void SetViewport(int x, int y, int width, int height);
    
    // Creating resources
    public IShader CreateShader(ShaderSettings settings);
    public ITexture CreateTexture(TextureSettings settings);
    public IVertexBuffer CreateVertexBuffer<T>(VertexBufferSettings<T> settings) where T : unmanaged;
    public IIndexBuffer CreateIndexBuffer<T>(IndexBufferSettings<T> settings)    where T : unmanaged;
    public IFrameBuffer CreateFrameBuffer(FrameBufferSettings settings);
    
    // Deleting resources
    public void DeleteShader(IShader shader);
    public void DeleteTexture(ITexture texture);
    public void DeleteVertexBuffer<T>(IVertexBuffer buffer) where T : unmanaged;
    public void DeleteIndexBuffer<T>(IIndexBuffer buffer) where T : unmanaged;
    public void DeleteFrameBuffer(IFrameBuffer buffer);
    
    // Updating resources
    public void UpdateVertexBuffer<T>(IVertexBuffer buffer, T[] data) where T : unmanaged;
    public void UpdateIndexBuffer<T>(IIndexBuffer buffer, T[] data)   where T : unmanaged;
    
    // Unbinding resources
    public void UnbindVertexBuffer();
    public void UnbindIndexBuffer();
    public void UnbindFrameBuffer();
    public GraphicsError? GetError();
}

public static class GraphicsApiExtensions
{
    public static bool SetUniform<T>(this IShader shader, IGraphicsApi api, string name, T value)
    {
        return api.SetUniform(shader, name, value);
    }
}