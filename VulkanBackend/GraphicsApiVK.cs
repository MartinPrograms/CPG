using Silk.NET.Vulkan;
using System.Numerics;
using CPG.Common;
using CPG.Common.Rendering;
using CPG.Interface;
using IWindow = Silk.NET.Windowing.IWindow;

namespace VulkanBackend;

public class GraphicsApiVK : IGraphicsApi
{
    private IWindow _window;
    private Vk _vk;
    
    public GraphicsApiVK(IWindow window)
    {
        _window = window;
        _vk = Vk.GetApi();
    }
    
    public void Init()
    {
        
    }

    public void Shutdown()
    {
        throw new NotImplementedException();
    }

    public void Clear(ClearMask mask)
    {
        throw new NotImplementedException();
    }

    public void SetClearColor(Vector4 color)
    {
        throw new NotImplementedException();
    }

    public void UseShader(IShader shader)
    {
        throw new NotImplementedException();
    }

    public bool SetUniform<T>(IShader shader, string name, T value)
    {
        throw new NotImplementedException();
    }

    public void UseTexture(ITexture texture, uint slot)
    {
        throw new NotImplementedException();
    }

    public void UseVertexBuffer<T>(IVertexBuffer buffer) where T : unmanaged
    {
        throw new NotImplementedException();
    }

    public void UseIndexBuffer<T>(IIndexBuffer buffer) where T : unmanaged
    {
        throw new NotImplementedException();
    }

    public void UseFrameBuffer(IFrameBuffer buffer)
    {
        throw new NotImplementedException();
    }

    public void Draw<V, I>(DrawCallSettings<V, I> settings) where V : unmanaged where I : unmanaged
    {
        throw new NotImplementedException();
    }

    public void SetViewport(int x, int y, int width, int height)
    {
        throw new NotImplementedException();
    }

    public IShader CreateShader(ShaderSettings settings)
    {
        throw new NotImplementedException();
    }

    public ITexture CreateTexture(TextureSettings settings)
    {
        throw new NotImplementedException();
    }

    public IVertexBuffer CreateVertexBuffer<T>(VertexBufferSettings<T> settings) where T : unmanaged
    {
        throw new NotImplementedException();
    }

    public IIndexBuffer CreateIndexBuffer<T>(IndexBufferSettings<T> settings) where T : unmanaged
    {
        throw new NotImplementedException();
    }

    public IFrameBuffer CreateFrameBuffer(FrameBufferSettings settings)
    {
        throw new NotImplementedException();
    }

    public void DeleteShader(IShader shader)
    {
        throw new NotImplementedException();
    }

    public void DeleteTexture(ITexture texture)
    {
        throw new NotImplementedException();
    }

    public void DeleteVertexBuffer<T>(IVertexBuffer buffer) where T : unmanaged
    {
        throw new NotImplementedException();
    }

    public void DeleteIndexBuffer<T>(IIndexBuffer buffer) where T : unmanaged
    {
        throw new NotImplementedException();
    }

    public void DeleteFrameBuffer(IFrameBuffer buffer)
    {
        throw new NotImplementedException();
    }

    public void UpdateVertexBuffer<T>(IVertexBuffer buffer, T[] data) where T : unmanaged
    {
        throw new NotImplementedException();
    }

    public void UpdateIndexBuffer<T>(IIndexBuffer buffer, T[] data) where T : unmanaged
    {
        throw new NotImplementedException();
    }

    public void UnbindVertexBuffer()
    {
        throw new NotImplementedException();
    }

    public void UnbindIndexBuffer()
    {
        throw new NotImplementedException();
    }

    public void UnbindFrameBuffer()
    {
        throw new NotImplementedException();
    }

    public GraphicsError? GetError()
    {
        throw new NotImplementedException();
    }
}