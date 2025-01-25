using System.Runtime.InteropServices;
using CPG.Common.Rendering;
using CPG.Interface;
using Silk.NET.OpenGL;

namespace OpenGLBackend;

// EBO.
public class IndexBufferGL<T> : IIndexBuffer where T : unmanaged
{
    private uint _handle;
    private GL _gl;
    public int Count { get; set; }
    private IndexBufferSettings<T> _settings;

    public unsafe IndexBufferGL(GL gl, IndexBufferSettings<T> settings)
    {
        _gl = gl;
        _handle = _gl.GenBuffer();
        _settings = settings;

        Count = settings.Count;
        var size = settings.Buffer.Size; // Total size of the buffer in bytes
        var count = settings.Count; // Number of indices
        
        BufferUsageARB usageARB = settings.Buffer.Settings.Usage switch
        {
            BufferUsage.Static => BufferUsageARB.StaticDraw,
            BufferUsage.Dynamic => BufferUsageARB.DynamicDraw,
            _ => BufferUsageARB.StaticDraw
        };
        
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _handle);
        void* data = settings.Buffer.Data;
        
        _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (uint)size, data, usageARB);
    }

    public void Bind()
    {
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _handle);
        
    }

    public void Dispose()
    {
        _gl.DeleteBuffer(_handle);
    }

    public unsafe void Update<T>(T[] data) where T : unmanaged
    {
        var size = data.Length * Marshal.SizeOf<T>();
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _handle);
        BufferUsageARB usageARB = _settings.Buffer.Settings.Usage switch
        {
            BufferUsage.Static => BufferUsageARB.StaticDraw,
            BufferUsage.Dynamic => BufferUsageARB.DynamicDraw,
            _ => BufferUsageARB.StaticDraw
        };
        fixed (void* ptr = data)
        {
            _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (uint)size, ptr,usageARB );
        }
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

    }
}