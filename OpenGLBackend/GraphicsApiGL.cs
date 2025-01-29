using System.Numerics;
using CPG.Common;
using CPG.Common.Rendering;
using CPG.Interface;
using Silk.NET.OpenGL;

namespace OpenGLBackend;

public class GraphicsApiGL : IGraphicsApi
{
    private GL _gl;
    public GraphicsApiGL(GL gl)
    {
        _gl = gl;
    }
    
    public void Init()
    {
        _gl.Enable(EnableCap.DepthTest);
        _gl.Enable(EnableCap.Blend);
        _gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        
        _gl.Enable(EnableCap.CullFace);
        _gl.CullFace(GLEnum.Back);
    }

    public void Shutdown()
    {
        // Remove all resources.
        // TODO: Do this lol, itll automatically be done by the GC but its good practice to do it manually.
    }

    public void BeginRenderPass(RenderPassInfo renderPassInfo)
    {
        
    }

    public void EndRenderPass()
    {
        // Useless on OpenGL, but VERY useful on Vulkan.
    }

    public IPipeline CreatePipeline(PipelineSettings settings)
    {
        throw new NotImplementedException();
    }

    public void BindPipeline(IPipeline pipeline)
    {
        throw new NotImplementedException();
    }

    public void DeletePipeline(IPipeline pipeline)
    {
        throw new NotImplementedException();
    }

    public void BindVertexBuffers(IVertexBuffer[] buffers, uint[] offsets)
    {
        throw new NotImplementedException();
    }

    public void BindIndexBuffer(IIndexBuffer buffer, uint offset)
    {
        throw new NotImplementedException();
    }

    public void BindDescriptorSets(IDescriptorSet[] descriptorSets, uint firstSet = 0)
    {
        throw new NotImplementedException();
    }

    public void Draw<V, I>(DrawCallSettings<V, I> drawInfo) where V : unmanaged where I : unmanaged
    {
        throw new NotImplementedException();
    }

    public IDescriptorSet CreateDescriptorSet(DescriptorSetSettings settings)
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

    public void DeleteDescriptorSet(IDescriptorSet descriptorSet)
    {
        throw new NotImplementedException();
    }

    public void DeleteTexture(ITexture texture)
    {
        throw new NotImplementedException();
    }

    public void DeleteVertexBuffer(IVertexBuffer buffer)
    {
        throw new NotImplementedException();
    }

    public void DeleteIndexBuffer(IIndexBuffer buffer)
    {
        throw new NotImplementedException();
    }

    public void DeleteFrameBuffer(IFrameBuffer buffer)
    {
        throw new NotImplementedException();
    }

    public void UpdateBuffer<T>(GpuBuffer<T> buffer, T[] data) where T : unmanaged
    {
        throw new NotImplementedException();
    }

    public GraphicsError? GetLastError()
    {
        var error = _gl.GetError();
        
        if (error == GLEnum.NoError)
        {
            return null;
        }
        
        var message = error switch
        {
            GLEnum.InvalidEnum => "Invalid Enum",
            GLEnum.InvalidValue => "Invalid Value",
            GLEnum.InvalidOperation => "Invalid Operation",
            GLEnum.StackOverflow => "Stack Overflow",
            GLEnum.StackUnderflow => "Stack Underflow",
            GLEnum.OutOfMemory => "Out of Memory",
            GLEnum.InvalidFramebufferOperation => "Invalid Framebuffer Operation",
            _ => "Unknown Error"
        };
        
        return new GraphicsError(message + " " + error);
    }
}