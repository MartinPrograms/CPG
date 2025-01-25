using CPG.Common.Rendering;
using CPG.Interface;
using Silk.NET.OpenGL;

namespace OpenGLBackend;

public class TextureManager
{
    public static List<ITexture> Textures = new();

    public static ITexture CreateTexture(GL gl, TextureSettings settings)
    {
        var texture = new TextureGL(gl, settings);
        Textures.Add(texture);
        return texture;
    }

    public static void DeleteTexture(ITexture texture)
    {
        if (texture is TextureGL glTexture)
        {
            glTexture.Dispose();
            Textures.Remove(texture);
        }
    }

    public static void UseTexture(ITexture texture, uint slot)
    {
        if (texture is TextureGL glTexture)
        {
            glTexture.Bind(slot);
        }
    }
}