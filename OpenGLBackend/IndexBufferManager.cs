using CPG.Common.Rendering;
using CPG.Interface;
using Silk.NET.OpenGL;

namespace OpenGLBackend;

public class IndexBufferManager
{
    public static List<IIndexBuffer> IndexBuffers = new();
    public static IIndexBuffer CreateIndexBuffer<T>(GL gl, IndexBufferSettings<T> settings) where T : unmanaged
    {
        var indexBuffer = new IndexBufferGL<T>(gl, settings);
        IndexBuffers.Add(indexBuffer);
        return indexBuffer;
    }

    public static void UseIndexBuffer<T>(IIndexBuffer buffer) where T : unmanaged
    {
        if (buffer is IndexBufferGL<T> glBuffer)
        {
            glBuffer.Bind();
        }
    }

    public static void DeleteIndexBuffer<T>(IIndexBuffer buffer) where T : unmanaged
    {
        if (buffer is IndexBufferGL<T> glBuffer)
        {
            glBuffer.Dispose();
            IndexBuffers.Remove(buffer);
        }
    }

    public static void UpdateIndexBuffer<T>(IIndexBuffer buffer, T[] data) where T : unmanaged
    {
        if (buffer is IndexBufferGL<T> glBuffer)
        {
            glBuffer.Update(data);
        }
    }

    public static void UnbindIndexBuffer(GL gl)
    {
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
    }
}