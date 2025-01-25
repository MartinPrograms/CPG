using CPG.Common.Rendering;
using CPG.Interface;
using Silk.NET.OpenGL;
using PixelFormat = Silk.NET.OpenGL.PixelFormat;

namespace OpenGLBackend;

public class TextureGL : ITexture
{
    private GL _gl;
    public uint ID { get; set; }

    public TextureGL(GL gl, TextureSettings settings)
    {
        _gl = gl;

        CreateTexture(settings);
    }

    private unsafe void CreateTexture(TextureSettings settings)
    {
        ID = _gl.GenTexture();
        
        _gl.BindTexture(GLEnum.Texture2D, ID);

        TextureWrapMode wrapMode;
        switch (settings.Wrap)
        {
            case TextureWrap.Repeat:
                wrapMode = TextureWrapMode.Repeat;
                break;
            case TextureWrap.MirroredRepeat:
                wrapMode = TextureWrapMode.MirroredRepeat;
                break;
            case TextureWrap.ClampToEdge:
                wrapMode = TextureWrapMode.ClampToEdge;
                break;
            case TextureWrap.ClampToBorder:
                wrapMode = TextureWrapMode.ClampToBorder;
                break;
            default:
                wrapMode = TextureWrapMode.Repeat;
                break;
        }
        
        _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)wrapMode);
        _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)wrapMode);
        
        TextureMagFilter magFilter;
        TextureMinFilter minFilter;
        
        switch (settings.Filter)
        {
            case TextureFilter.Nearest:
                magFilter = TextureMagFilter.Nearest;
                minFilter = TextureMinFilter.Nearest;
                break;
            case TextureFilter.Linear:
                magFilter = TextureMagFilter.Linear;
                minFilter = TextureMinFilter.Linear;
                break;
            case TextureFilter.NearestMipmapNearest:
                magFilter = TextureMagFilter.Nearest;
                minFilter = TextureMinFilter.NearestMipmapNearest;
                break;
            case TextureFilter.LinearMipmapNearest:
                magFilter = TextureMagFilter.Linear;
                minFilter = TextureMinFilter.LinearMipmapNearest;
                break;
            case TextureFilter.NearestMipmapLinear:
                magFilter = TextureMagFilter.Nearest;
                minFilter = TextureMinFilter.NearestMipmapLinear;
                break;
            case TextureFilter.LinearMipmapLinear:
                magFilter = TextureMagFilter.Linear;
                minFilter = TextureMinFilter.LinearMipmapLinear;
                break;
            default:
                magFilter = TextureMagFilter.Linear;
                minFilter = TextureMinFilter.Linear;
                break;
        }
        
        _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)magFilter);
        _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)minFilter);

        PixelFormat format;
        switch (settings.Format)
        {
            case TextureFormat.R:
                format = PixelFormat.Red;
                break;
            case TextureFormat.RG:
                format = PixelFormat.RG;
                break;
            case TextureFormat.RGB:
                format = PixelFormat.Rgb;
                break;
            case TextureFormat.RGBA:
                format = PixelFormat.Rgba;
                break; 
            case TextureFormat.Depth:
                format = PixelFormat.DepthComponent;
                break;
            default:
                format = PixelFormat.Rgba;
                break;
        }
        
        PixelType type;
        switch (settings.PixelFormat)
        {
            case CPG.Common.Rendering.PixelFormat.UnsignedByte:
                type = PixelType.UnsignedByte;
                break;
            case CPG.Common.Rendering.PixelFormat.Float:
                type = PixelType.Float;
                break;
            default:
                type = PixelType.UnsignedByte;
                break;
        }
        
        _gl.TexImage2D(GLEnum.Texture2D, 0, (int)format, (uint) settings.Width, (uint) settings.Height, 0, format, type, null);

        if (settings.Data != null) // settings.Data is an IntPtr
        {
            _gl.TexSubImage2D(GLEnum.Texture2D, 0, 0, 0, (uint) settings.Width, (uint) settings.Height, format, type, (void*)settings.Data);
        }
        
        if (settings.GenerateMipmaps)
        {
            _gl.GenerateMipmap(GLEnum.Texture2D);
        }
        
        // We are done with the texture, unbind it.
        
        _gl.BindTexture(GLEnum.Texture2D, 0);
    }
    public void Bind(uint slot)
    {
        _gl.ActiveTexture(TextureUnit.Texture0 + (int)slot);
        _gl.BindTexture(GLEnum.Texture2D, ID);
    }
    
    public void Dispose()
    {
        _gl.DeleteTexture(ID);
    }
}