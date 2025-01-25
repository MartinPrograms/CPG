using CPG.Common;
using CPG.Interface;
using CPG.Interface.Settings;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using IWindow = CPG.Interface.IWindow;

namespace OpenGLBackend;

public class WindowGL : IWindow
{
    public string Title { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool Fullscreen { get; set; }
    public bool VSync { get; set; }
    public double DeltaTime { get; set; }
    public double Time { get; set; }
    public bool NeedsSwap { get; set; }
    
    private Silk.NET.Windowing.IWindow _window;
    private WindowState _windowState;

    public IInput Input { get; private set; }

    public WindowGL(WindowSettings settings)
    {
        Title = settings.Title;
        Width = settings.Width;
        Height = settings.Height;
        Fullscreen = settings.Fullscreen;
        VSync = settings.VSync;
        NeedsSwap = true;
        
        Window.PrioritizeGlfw();
        
        _window = Window.Create(WindowOptions.Default with
        {
            Title = Title,
            Size = new(Width, Height),
            WindowState = Fullscreen ? WindowState.Fullscreen : WindowState.Normal,
            VSync = VSync,
            API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.Default,new APIVersion(3,3)),
            ShouldSwapAutomatically = false
        });
        
        _windowState = Fullscreen ? WindowState.Fullscreen : WindowState.Normal;
        
        _window.Render += (a) =>
        {
            this.Time += a;
            this.DeltaTime = a;
            
            _renderCallback?.Render(GraphicsApi, this);
            
            // Check for errors
            var error = GraphicsApi.GetError();
            if (error != null)
            {
                throw new GraphicsException(error);
            }
        };
        
        _window.Update += (a) =>
        {
            Input.Update();
            _updateCallback?.Invoke(this);
        
        };

        _window.Load += () =>
        {
            var api = _window.CreateOpenGL();
            var input = _window.CreateInput();
            GraphicsApi = new GraphicsApiGL(api);
            Input = new InputSilk(input);
        
            _loadCallback?.Invoke(this);
        };

        _window.FramebufferResize += a =>
        {
            var x = a.X;
            var y = a.Y;

            Width = x;
            Height = y;
        };
    }
    
    public void Show()
    {
        _window.WindowState = _windowState;
        _window.VSync = VSync;
        _window.Run();
    }

    public void Hide()
    {
        _window.WindowState = WindowState.Minimized;
    }

    public void Close()
    {
        _window.Close();
    }
    
    private GraphicsCallback _renderCallback;
    private Action<IWindow> _updateCallback;
    private Action<IWindow> _loadCallback;

    public IGraphicsApi GraphicsApi { get; set; }
    public void SetRenderCallback(GraphicsCallback callback)
    {
        _renderCallback = callback;
    }

    public void SetUpdateCallback(Action<IWindow> callback)
    {
        _updateCallback = callback;
    }
    
    public void SetLoadCallback(Action<IWindow> action)
    {
        _loadCallback = action;
    }
    
    public void SwapBuffers()
    {
        if (NeedsSwap)   
            _window.SwapBuffers();
    }
}