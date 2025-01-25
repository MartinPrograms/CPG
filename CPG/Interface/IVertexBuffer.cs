namespace CPG.Interface;

public interface IVertexBuffer
{
    /// <summary>
    /// T is the type of the index buffer.
    /// </summary>
    /// <param name="indexBuffer"></param>
    /// <typeparam name="T"></typeparam>
    public void LinkIndexBuffer<T>(IIndexBuffer indexBuffer) where T : unmanaged;
}