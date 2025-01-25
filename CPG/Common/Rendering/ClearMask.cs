namespace CPG.Common.Rendering;

[Flags]
public enum ClearMask
{
    Color = 1,
    Depth = 2,
    Stencil = 4
}