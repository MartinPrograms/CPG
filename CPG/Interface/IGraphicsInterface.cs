using CPG.Common;
using CPG.Interface.Settings;

namespace CPG.Interface;

/// <summary>
/// This class is an interface for the graphics system.
/// It is the topmost class for the graphics system.
/// Below this you have the window, which creates a window, and that window also creates a renderer.
///
/// It's the same for OpenGL, DirectX, Vulkan, etc. The only difference is the implementation.
/// </summary>
public interface IGraphicsInterface
{
    public string Name { get; }
    
    public GraphicsSystem SupportedSystem { get; }
    public bool IsSupported();
    public bool MultiThreaded { get; }
    public bool MultiWindow { get; }
    public SettingsContainer Settings { get; set; }
    
    public IWindow Create(WindowSettings settings);
}