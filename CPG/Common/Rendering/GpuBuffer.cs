using CPG.Interface;

namespace CPG.Common.Rendering;

public unsafe class GpuBuffer<T> : IBuffer
{
    public void* Data { get; }
    /// <summary>
    /// Size of T[] in bytes, so for example 32 * 4 for a 32 float array
    /// </summary>
    public uint Size { get; }
    public int Count { get; }
    public GpuBufferSettings Settings { get; }
    
    public GpuBuffer(void* data, uint size, int count, GpuBufferSettings settings)
    {
        Data = data;
        Size = size;
        Count = count;
        Settings = settings;
    }
}

public struct GpuBufferSettings
{
    public BufferUsage Usage;
    public BufferAccess Access;
    
    public GpuBufferSettings(BufferUsage usage, BufferAccess access)
    {
        Usage = usage;
        Access = access;
    }
}

public enum BufferUsage
{
    Static,
    Dynamic
}

public enum BufferAccess
{
    ReadOnly,
    WriteOnly,
    ReadWrite
}