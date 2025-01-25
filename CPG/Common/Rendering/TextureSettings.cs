namespace CPG.Common.Rendering;

public record TextureSettings(int Width,
    int Height,
    TextureFormat Format,
    PixelFormat PixelFormat,
    TextureFilter Filter,
    TextureWrap Wrap,
    bool GenerateMipmaps,
    IntPtr? Data = null
    );

public enum TextureFormat
{
    R,
    RG,
    RGB,
    RGBA,
    Depth
}

public enum PixelFormat
{
    UnsignedByte,
    Float
}

public enum TextureFilter
{
    Nearest,
    Linear,
    NearestMipmapNearest,
    LinearMipmapNearest,
    NearestMipmapLinear,
    LinearMipmapLinear
    
}

public enum TextureWrap
{
    None = 0,
    Repeat = 1,
    MirroredRepeat = 2,
    ClampToEdge = 4,
    ClampToBorder = 8
}