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

    public void Clear(ClearMask mask)
    {
        var clearMask = 0;
        if (mask.HasFlag(ClearMask.Color))
        {
            clearMask |= (int)GLEnum.ColorBufferBit;
        }
        
        if (mask.HasFlag(ClearMask.Depth))
        {
            clearMask |= (int)GLEnum.DepthBufferBit;
        }
        
        if (mask.HasFlag(ClearMask.Stencil))
        {
            clearMask |= (int)GLEnum.StencilBufferBit;
        }
        
        _gl.Clear((uint)clearMask);
    }

    public void SetClearColor(Vector4 color)
    {
        _gl.ClearColor(color.X, color.Y, color.Z, color.W);
    }

    public void UseShader(IShader shader)
    {
        ShaderManager.UseShader(shader);
    }

    public bool SetUniform<T>(IShader shader, string name, T value)
    {
        return ShaderManager.SetUniform(_gl, shader, name, value);
    }

    public void UseTexture(ITexture texture, uint slot)
    {
        TextureManager.UseTexture(texture, slot);
    }

    public void UseVertexBuffer<T>(IVertexBuffer buffer) where T : unmanaged
    {
        VertexBufferManager.UseVertexBuffer<T>(buffer);
    }

    public void UseIndexBuffer<T>(IIndexBuffer buffer) where T : unmanaged
    {
        IndexBufferManager.UseIndexBuffer<T>(buffer);
    }

    public void UseFrameBuffer(IFrameBuffer buffer)
    {
        throw new System.NotImplementedException();
        //FrameBufferManager.UseFrameBuffer(buffer);
    }

    public unsafe void Draw<V,I>(DrawCallSettings<V,I> settings) where V : unmanaged where I : unmanaged
    {
        var type = settings.PrimitiveType switch
        {
            Primitive.Triangle => PrimitiveType.Triangles,
            Primitive.Line => PrimitiveType.Lines,
            Primitive.Quad => PrimitiveType.Points,
            _ => PrimitiveType.Triangles
        };
        
        var instanceCount = settings.InstanceCount;
        var indexCount = settings.IndexCount;
        
        var indexBuffer = settings.IndexBuffer; // nullable
        var vertexBuffer = settings.VertexBuffer;
        
        UseVertexBuffer<V>(vertexBuffer);
        
        if (indexBuffer != null)
        {
            UseIndexBuffer<I>(indexBuffer);
            
            if (instanceCount > 0)
            {
                _gl.DrawElementsInstanced(type, (uint)indexCount, GLEnum.UnsignedInt, null, (uint)instanceCount);
            }
            else
            {
                _gl.DrawElements(type, (uint)indexCount, GLEnum.UnsignedInt, null);
            }
        }
        else
        {
            
            if (instanceCount > 0)
            {
                _gl.DrawArraysInstanced(type, 0, (uint)indexCount, (uint)instanceCount);
            }
            else
            {
                _gl.DrawArrays(type, 0, (uint)indexCount);
            }
        }
    }

    public void SetViewport(int x, int y, int width, int height)
    {
        _gl.Viewport(x, y, (uint)width, (uint)height);
    }

    public IShader CreateShader(ShaderSettings settings)
    {
        return ShaderManager.CreateShader(_gl, settings);
    }

    public ITexture CreateTexture(TextureSettings settings)
    {
        return TextureManager.CreateTexture(_gl, settings);
    }

    public IVertexBuffer CreateVertexBuffer<T>(VertexBufferSettings<T> settings) where T : unmanaged
    {
        return VertexBufferManager.CreateVertexBuffer(_gl, settings);
    }

    public IIndexBuffer CreateIndexBuffer<T>(IndexBufferSettings<T> settings) where T : unmanaged
    {
        return IndexBufferManager.CreateIndexBuffer(_gl, settings);
    }

    public IFrameBuffer CreateFrameBuffer(FrameBufferSettings settings)
    {
        throw new System.NotImplementedException();
        //return FrameBufferManager.CreateFrameBuffer(settings);
    }

    public void DeleteShader(IShader shader)
    {
        ShaderManager.DeleteShader(shader);
    }

    public void DeleteTexture(ITexture texture)
    {
        TextureManager.DeleteTexture(texture);
    }

    public void DeleteVertexBuffer<T>(IVertexBuffer buffer) where T : unmanaged
    {
        VertexBufferManager.DeleteVertexBuffer<T>(buffer);
    }

    public void DeleteIndexBuffer<T>(IIndexBuffer buffer) where T : unmanaged
    {
        IndexBufferManager.DeleteIndexBuffer<T>(buffer);
    }

    public void DeleteFrameBuffer(IFrameBuffer buffer)
    {
        throw new System.NotImplementedException();
        //FrameBufferManager.DeleteFrameBuffer(buffer);
    }

    public void UpdateVertexBuffer<T>(IVertexBuffer buffer, T[] data) where T : unmanaged
    {
        VertexBufferManager.UpdateVertexBuffer(buffer, data);
    }

    public void UpdateIndexBuffer<T>(IIndexBuffer buffer, T[] data) where T : unmanaged
    {
        IndexBufferManager.UpdateIndexBuffer(buffer, data);
    }

    public void UnbindVertexBuffer()
    {
        VertexBufferManager.UnbindVertexBuffer(_gl);
    }

    public void UnbindIndexBuffer()
    {
        IndexBufferManager.UnbindIndexBuffer(_gl);
    }

    public void UnbindFrameBuffer()
    {
        throw new System.NotImplementedException();
        //FrameBufferManager.UnbindFrameBuffer();
    }

    public GraphicsError? GetError()
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