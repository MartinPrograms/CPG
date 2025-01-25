using StbImageSharp;
using CPG.Common.Rendering;
using CPG.Interface;

namespace Engine;

public class Texture
{
    private IGraphicsApi _graphicsApi;
    private ITexture _texture;
    public unsafe Texture(IGraphicsApi objGraphicsApi, string path)
    {
        _graphicsApi = objGraphicsApi;
        
        StbImage.stbi_set_flip_vertically_on_load(1); // Flip the image vertically, OpenGL expects the origin to be at the bottom left
        var result = ImageResult.FromMemory(File.ReadAllBytes(path), ColorComponents.RedGreenBlueAlpha);
        
        var textureSettings = new TextureSettings(result.Width, result.Height, TextureFormat.RGBA,
            PixelFormat.UnsignedByte, TextureFilter.Nearest, TextureWrap.Repeat, false, result.DataPtr);
        
        _texture = _graphicsApi.CreateTexture(textureSettings);
    }
    
    public void Use(uint slot)
    {
        _graphicsApi.UseTexture(_texture, slot);
    }
}