namespace CPG.Interface;

/// <summary>
/// This interface is a window. It's used to create a window in the graphics system.
/// It also contains the render callback, and the update callback.
/// </summary>
public interface IWindow
{
    public string Title { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool Fullscreen { get; set; }
    public bool VSync { get; set; }
    public bool NeedsSwap { get; set; }
    
    public double DeltaTime { get; }
    public double Time { get; }
    
    public IInput Input { get; }
    
    /// <summary>
    /// Blocking call, it will show the window.
    /// </summary>
    public void Show();
    public void Hide();
    public void Close();
    
    /// <summary>
    /// Internal, because it's only used by the graphics system, it should NOT be used in the update callback.
    /// </summary>
    public IGraphicsApi GraphicsApi { get; }
    
    /// <summary>
    /// A callback that is called every frame, includes the IGraphicsApi and the IWindow.
    /// </summary>
    /// <param name="callback"></param>
    public void SetRenderCallback(GraphicsCallback callback);
    
    /// <summary>
    /// Update callback, if multithreaded might be called on a different thread.
    /// It is important it's not calling any graphics functions, or other thread-unsafe functions.
    /// </summary>
    public void SetUpdateCallback(Action<IWindow> callback);
    
    /// <summary>
    /// Load callback, called once when the window is created.
    /// This is where you should load assets, if they're expected to be in the first frame.
    /// You *can* use window.GraphicsApi here.
    /// </summary>
    /// <param name="action"></param>
    public void SetLoadCallback(Action<IWindow> action);
    
    public void SwapBuffers();
}