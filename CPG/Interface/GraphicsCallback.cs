namespace CPG.Interface;

public class GraphicsCallback
{
    public Action<IGraphicsApi,IWindow> RenderCallback { get; }
    
    public GraphicsCallback(Action<IGraphicsApi,IWindow> renderCallback)
    {
        RenderCallback = renderCallback;
    }
    
    public void Render(IGraphicsApi graphicsApi, IWindow window)
    {
        RenderCallback(graphicsApi, window);
    }
}