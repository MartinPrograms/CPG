using CPG.Common.Rendering;
using CPG.Interface;
using Silk.NET.OpenGL;

namespace OpenGLBackend;

public class VertexBufferManager
{
    private static List<IVertexBuffer> VertexBuffers = new();
    public static IVertexBuffer CreateVertexBuffer<T>(GL gl, VertexBufferSettings<T> settings) where T : unmanaged
    {
        var a =  new VertexBufferGL<T>(gl, settings);
        
        VertexBuffers.Add(a);
        return a;
    }

    public static void UseVertexBuffer<T>(IVertexBuffer buffer) where T : unmanaged
    { 
        if (buffer is VertexBufferGL<T> glBuffer)
        {
            glBuffer.Bind();
        }
    }

    public static void DeleteVertexBuffer<T>(IVertexBuffer buffer) where T : unmanaged
    {
        if (buffer is VertexBufferGL<T> glBuffer)
        {
            glBuffer.Dispose();
            VertexBuffers.Remove(buffer);
        }
    }

    public static void UpdateVertexBuffer<T>(IVertexBuffer buffer, T[] data) where T : unmanaged
    {
        if (buffer is VertexBufferGL<T> glBuffer)
        {
            glBuffer.Update(data);
        }
    }

    public static void UnbindVertexBuffer(GL gl)
    {
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        gl.BindVertexArray(0);
    }
}