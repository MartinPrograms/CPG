namespace CPG.Common.Rendering;

public struct IndexBufferSettings<T> where T : unmanaged
{
    public GpuBuffer<T> Buffer;
    public int Count;
    
    public IndexBufferSettings(GpuBuffer<T> buffer, int count)
    {
        Buffer = buffer;
        Count = count;
    }
}