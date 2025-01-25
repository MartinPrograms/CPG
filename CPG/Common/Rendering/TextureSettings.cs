namespace CPG.Common.Rendering;

public unsafe class TextureSettings
{
    public int Width;
    public int Height;
    public     TextureFormat Format;
    public PixelFormat PixelFormat;
    public     TextureFilter Filter;
    public TextureWrap Wrap;
    public bool GenerateMipmaps;
    public void* Data = null;
    
    public TextureSettings(int width, int height, TextureFormat format, PixelFormat pixelFormat, TextureFilter filter, TextureWrap wrap, bool generateMipmaps, void* data = null)
    {
        Width = width;
        Height = height;
        Format = format;
        PixelFormat = pixelFormat;
        Filter = filter;
        Wrap = wrap;
        GenerateMipmaps = generateMipmaps;
        Data = data;
    }
};

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