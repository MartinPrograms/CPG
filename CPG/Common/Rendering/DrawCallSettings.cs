using CPG.Interface;

namespace CPG.Common.Rendering;

public record DrawCallSettings<V,I>(IVertexBuffer VertexBuffer, IIndexBuffer? IndexBuffer, Primitive PrimitiveType, int VertexCount, int IndexCount, int InstanceCount) where V : unmanaged where I : unmanaged;

public enum Primitive
{
    Triangle,
    Quad,
    Line
}