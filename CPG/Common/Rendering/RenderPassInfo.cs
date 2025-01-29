using System.Numerics;
using CPG.Interface;

namespace CPG.Common.Rendering;

public class RenderPassInfo
{
    public IFrameBuffer FrameBuffer { get; set; }
    public ClearValue[] ClearValues { get; set; }
    public Vector4 RenderArea { get; set; }
    public LoadOp LoadOp { get; set; }
    public StoreOp StoreOp { get; set; }
    public LoadOp StencilLoadOp { get; set; }
    public StoreOp StencilStoreOp { get; set; }
    public bool StencilTestEnable { get; set; }
    public bool DepthTestEnable { get; set; }
    public bool DepthWriteEnable { get; set; }
}

public enum LoadOp
{
    Load,
    Clear,
    DontCare
}

public enum StoreOp
{
    Store,
    DontCare
}

public class ClearValue
{
    public ClearMask Mask { get; set; }
    public Vector4 Color { get; set; }
}