using System.Runtime.InteropServices;
using CPG.Common;
using CPG.Interface;
using CPG.Interface.Settings;
using Silk.NET.Core.Native;
using Silk.NET.Input;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;
using IWindow = CPG.Interface.IWindow;

namespace VulkanBackend;

public class WindowVK : IWindow
{
    public string Title { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public bool Fullscreen
    {
        get => _windowState == WindowState.Fullscreen;
        set => _windowState = value ? WindowState.Fullscreen : WindowState.Normal;
    }

    public bool VSync { get; set; }
    public double DeltaTime { get; set; }
    public double Time { get; set; }
    public bool NeedsSwap { get; set; }
    
    private Silk.NET.Windowing.IWindow _window;
    private WindowState _windowState;

    public IInput Input { get; private set; }

    internal SettingsContainer Settings { get; set; }
    public WindowVK(WindowSettings settings, SettingsContainer settingsContainer)
    {
        Settings = settingsContainer;
        
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
            // Vulkan API
            API = new GraphicsAPI(ContextAPI.Vulkan, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(1,0)), 
            ShouldSwapAutomatically = false
        });
        
        _windowState = Fullscreen ? WindowState.Fullscreen : WindowState.Normal;
        
        _window.Render += (a) =>
        {
            this.Time += a;
            this.DeltaTime = a;
            
            GraphicsApiVK.BeginFrame();
            
            _renderCallback?.Render(GraphicsApi!, this);
            
            GraphicsApiVK.EndFrame();
            
            // No need to check for errors, the GraphicsApi will handle it.
            // Vulkan is nice like that.
        };
        
        _window.Update += (a) =>
        {
            Input.Update();
            _updateCallback?.Invoke(this);
        };

        _window.Load += () =>
        {
            var input = _window.CreateInput();
            GraphicsApi = new GraphicsApiVK(this);
            Input = new InputSilk(input);

            GraphicsApi.Init();
            
            _loadCallback?.Invoke(this);
        };

        _window.FramebufferResize += a =>
        {
            var x = a.X;
            var y = a.Y;

            Width = x;
            Height = y;
        };
        
        _window.Closing += () =>
        {
            GraphicsApi.Shutdown();
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
    internal GraphicsApiVK GraphicsApiVK => (GraphicsApiVK)GraphicsApi;
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
        
        if (_window.WindowState != _windowState)
            _window.WindowState = _windowState;
    }

    public unsafe string[] GetRequiredExtensions()
    {
        var ptr = _window.VkSurface.GetRequiredExtensions(out uint count);
        List<string> extensions = new();
        for (int i = 0; i < count; i++)
        {
            extensions.Add(Marshal.PtrToStringAnsi((IntPtr)ptr[i]));
        }
        
        return extensions.ToArray();
    }

    public unsafe SurfaceKHR CreateSurface(Instance instance)
    {
        var a = _window.VkSurface!.Create<SurfaceKHR>(instance.ToHandle(), null);
        return a.ToSurface();
    }

    public Extent2D Extent()
    {
        return new Extent2D((uint)Width, (uint)Height);
    }
}