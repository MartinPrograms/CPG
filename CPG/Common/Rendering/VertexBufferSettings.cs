namespace CPG.Common.Rendering;

public struct VertexBufferSettings<T> where T : unmanaged
{
    public GpuBuffer<T> Buffer;
    /// <summary>
    /// The stride of the vertex buffer in bytes.
    /// </summary>
    public uint Stride; 
    public VertexAttribute[] Attributes;
    
    public VertexBufferSettings(GpuBuffer<T> buffer, uint stride, VertexAttribute[] attributes)
    {
        Buffer = buffer;
        Stride = stride;
        Attributes = attributes;
    }
}

public struct VertexAttribute
{
    public uint Index;
    /// <summary>
    /// Size in elements, not bytes. Very important.
    /// </summary>
    public uint Size;
    public uint Offset;
    public VertexAttributeType Type;
    public bool Normalized;
    
    public VertexAttribute(uint index, uint size, uint offset, VertexAttributeType type, bool normalized)
    {
        Index = index;
        Size = size;
        Offset = offset;
        Type = type;
        Normalized = normalized;
    }
}

public enum VertexAttributeType
{
    Byte,
    UByte,
    Short,
    UShort,
    Int,
    UInt,
    Float,
    Double
}