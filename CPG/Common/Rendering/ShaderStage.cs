namespace CPG.Common.Rendering;

public record ShaderStage(ShaderStageType Type, string Source);

public enum ShaderStageType
{
    Vertex,
    Fragment,
    Compute
}