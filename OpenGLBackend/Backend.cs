using CPG.Common;
using CPG.Interface;
using CPG.Interface.Settings;
using OperatingSystem = CPG.Common.OperatingSystem;

namespace OpenGLBackend;

/// <summary>
/// OpenGL 3.3 backend for CPG.
/// </summary>
public class OpenGLBackend : IGraphicsInterface
{
    public string Name => "OpenGL";

    public GraphicsSystem SupportedSystem =>
        new GraphicsSystem(OperatingSystem.Windows | OperatingSystem.Linux | OperatingSystem.MacOS); // Because it's OpenGL 3.3, it should work on all platforms

    public bool IsSupported()
    {
        if (SupportedSystem.IsSupported())
            return true;
        
        return false;
    }

    public bool MultiThreaded => true;
    public bool MultiWindow => false;
    public SettingsContainer Settings { get; set; } = new();

    public IWindow Create(WindowSettings settings)
    {
        var window = new WindowGL(settings);
        return window;
    }
}