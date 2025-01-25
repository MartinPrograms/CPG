namespace CPG.Common.Rendering;

public struct FrameBufferSettings
{
    public int Width { get; }
    public int Height { get; }
    public int Samples { get; }
    public bool HasDepth { get; }
    public bool HasStencil { get; }
}