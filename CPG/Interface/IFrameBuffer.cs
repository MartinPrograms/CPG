namespace CPG.Interface;

public class IFrameBuffer
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int Samples { get; set; }
    
    public int ID { get; set; }
    
    public ITexture Texture { get; set; }
    public ITexture DepthTexture { get; set; }
    
    public IFrameBuffer(int width, int height, int samples)
    {
        Width = width;
        Height = height;
        Samples = samples;
    }
}