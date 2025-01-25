using System.Runtime.InteropServices;
using CPG.Common.Rendering;
using CPG.Interface;
using Silk.NET.OpenGL;

namespace OpenGLBackend;

public class VertexBufferGL<T> : IVertexBuffer where T : unmanaged
{
    private GL _gl;
    private VertexBufferSettings<T> _settings;
    private uint _vbo;
    private uint _vao;
    // The ebo is outside of this class. 
    public unsafe VertexBufferGL(GL gl, VertexBufferSettings<T> settings)
    {
        _gl = gl;
        _settings = settings;
        
        _vbo = _gl.GenBuffer();
        _vao = _gl.GenVertexArray();
        
        _gl.BindVertexArray(_vao);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        
        var buffer = _settings.Buffer;
        var size = buffer.Size; // In bytes.
        var data = buffer.Data; // The actual pointer to the data.
        var usage = buffer.Settings.Usage;
        var access = buffer.Settings.Access;
        
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        BufferUsageARB usageARB = usage switch
        {
            BufferUsage.Static => BufferUsageARB.StaticDraw,
            BufferUsage.Dynamic => BufferUsageARB.DynamicDraw,
            _ => BufferUsageARB.StaticDraw
        };
        
        BufferAccessARB accessARB = access switch // Unused for vertex buffers.
        {
            BufferAccess.ReadOnly => BufferAccessARB.ReadOnly,
            BufferAccess.WriteOnly => BufferAccessARB.WriteOnly,
            BufferAccess.ReadWrite => BufferAccessARB.ReadWrite,
            _ => BufferAccessARB.ReadWrite
        };
        
        _gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)size, data, usageARB);
        
        Helpers.CheckError(gl);
        
        var attributes = _settings.Attributes;
        var stride = _settings.Stride;
        
        Helpers.CheckError(gl);

        foreach (var attrib in attributes)
        {
            var normalized = attrib.Normalized;
            var index = attrib.Index;
            var sib = attrib.Size;
            var offset = attrib.Offset;
            var type = attrib.Type;
            
            VertexAttribPointerType typeARB = type switch
            {
                VertexAttributeType.Byte => VertexAttribPointerType.Byte,
                VertexAttributeType.UByte => VertexAttribPointerType.UnsignedByte,
                VertexAttributeType.Short => VertexAttribPointerType.Short,
                VertexAttributeType.UShort => VertexAttribPointerType.UnsignedShort,
                VertexAttributeType.Int => VertexAttribPointerType.Int,
                VertexAttributeType.UInt => VertexAttribPointerType.UnsignedInt,
                VertexAttributeType.Float => VertexAttribPointerType.Float,
                VertexAttributeType.Double => VertexAttribPointerType.Double, // 99% of hardware doesn't support this, but whatever.
                _ => VertexAttribPointerType.Float
            };
             
            _gl.VertexAttribPointer(index, (int)sib, typeARB, normalized, stride, (nint)offset);
            _gl.EnableVertexAttribArray(index);
            Helpers.CheckError(gl);

        }
        
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        
        // We do NOT unbind the buffer here, if indices are to be used, the buffer must be bound.
        // It is assumed the user will unbind the buffer.
        
        Helpers.CheckError(gl);
    }

    public void Bind()
    {
        _gl.BindVertexArray(_vao);
    }

    public void Dispose()
    {
        _gl.DeleteBuffer(_vbo);
        _gl.DeleteVertexArray(_vao);
    }

    public unsafe void Update<T>(T[] data) where T : unmanaged
    {
        var size = data.Length * Marshal.SizeOf<T>();
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        BufferUsageARB usageARB = _settings.Buffer.Settings.Usage switch
        {
            BufferUsage.Static => BufferUsageARB.StaticDraw,
            BufferUsage.Dynamic => BufferUsageARB.DynamicDraw,
            _ => BufferUsageARB.StaticDraw
        };
        fixed (void* ptr = data)
        {
            _gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)size, ptr,usageARB );
        }
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
    }

    public void LinkIndexBuffer<T>(IIndexBuffer indexBuffer) where T : unmanaged
    {
        if (indexBuffer is IndexBufferGL<T> glBuffer)
        {
            _gl.BindVertexArray(_vao);
            glBuffer.Bind();
            
            _gl.BindVertexArray(0);
        }
    }
}