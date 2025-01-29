using System.Numerics;

namespace CPG.Common.Rendering;

public class PipelineSettings
{
    ShaderStage[] ShaderStages;
    VertexAttribute[] VertexInput;
    InputAssemblyState InputAssembly;
    RasterizationState Rasterization;
    ViewportState Viewport;
    MultisampleState Multisampling;
    DepthStencilState DepthStencil;
    BlendState BlendState;
}

public class InputAssemblyState
{
    Primitive Topology; 
    bool PrimitiveRestartEnable;
}

public class RasterizationState
{
    bool DepthClampEnable;
    bool RasterizerDiscardEnable;
    PolygonMode PolygonMode; 
    CullModeFlags CullMode;
    FrontFace FrontFace;
    bool DepthBiasEnable;
    float DepthBiasConstantFactor;
    float DepthBiasClamp;
    float DepthBiasSlopeFactor;
    float LineWidth;
}

public enum PolygonMode
{
    Fill,
    Line,
    Point
}

public enum CullModeFlags
{
    None,
    Front,
    Back,
    FrontAndBack
}

public enum FrontFace
{
    CounterClockwise,
    Clockwise
}

public class ViewportState
{
    Viewport Viewport;
    Rect2D Scissors;
}

public class Viewport
{
    float X;
    float Y;
    float Width;
    float Height;
    float MinDepth;
    float MaxDepth;
}

public class Rect2D
{
    Offset2D Offset;
    Extent2D Extent;
}

public class Offset2D
{
    int X;
    int Y;
}

public class Extent2D
{
    int Width;
    int Height;
}

public class MultisampleState
{
    int RasterizationSamples;
    bool SampleShadingEnable;
    float MinSampleShading;
    SampleMask SampleMask;
    bool AlphaToCoverageEnable;
    bool AlphaToOneEnable;
}

public class SampleMask
{
    int[] SampleArray;
}

public class DepthStencilState
{
    bool DepthTestEnable;
    bool DepthWriteEnable;
    CompareOp DepthCompareOp;
    bool DepthBoundsTestEnable;
    bool StencilTestEnable;
    StencilOpState Front;
    StencilOpState Back;
    float MinDepthBounds;
    float MaxDepthBounds;
}

public class StencilOpState
{
    StencilOp FailOp;
    StencilOp PassOp;
    StencilOp DepthFailOp;
    CompareOp CompareOp;
    int CompareMask;
    int WriteMask;
    int Reference;
}

public enum StencilOp
{
    Keep,
    Zero,
    Replace,
    IncrementAndClamp,
    DecrementAndClamp,
    Invert,
    IncrementAndWrap,
    DecrementAndWrap
}

public enum CompareOp
{
    Never,
    Less,
    Equal,
    LessOrEqual,
    Greater,
    NotEqual,
    GreaterOrEqual,
    Always
}

public class BlendState
{
    bool LogicOpEnable;
    LogicOp LogicOp;
    Vector4 BlendConstants;
    BlendAttachmentState[] Attachments;
}

public enum LogicOp
{
    Clear,
    And,
    AndReverse,
    Copy,
    AndInverted,
    NoOp,
    Xor,
    Or,
    Nor,
    Equivalent,
    Invert,
    OrReverse,
    CopyInverted,
    OrInverted,
    Nand,
    Set
}

public class BlendAttachmentState
{
    bool BlendEnable;
    BlendFactor SrcColorBlendFactor;
    BlendFactor DstColorBlendFactor;
    BlendOp ColorBlendOp;
    BlendFactor SrcAlphaBlendFactor;
    BlendFactor DstAlphaBlendFactor;
    BlendOp AlphaBlendOp;
    ColorComponentFlags ColorWriteMask;
}

public enum BlendFactor
{
    Zero,
    One,
    SrcColor,
    OneMinusSrcColor,
    DstColor,
    OneMinusDstColor,
    SrcAlpha,
    OneMinusSrcAlpha,
    DstAlpha,
    OneMinusDstAlpha,
    ConstantColor,
    OneMinusConstantColor,
    ConstantAlpha,
    OneMinusConstantAlpha,
    SrcAlphaSaturate,
    Src1Color,
    OneMinusSrc1Color,
    Src1Alpha,
    OneMinusSrc1Alpha
}

public enum BlendOp
{
    Add,
    Subtract,
    ReverseSubtract,
    Min,
    Max
}

public enum ColorComponentFlags
{
    RBit = 0x1,
    GBit = 0x2,
    BBit = 0x4,
    ABit = 0x8
}