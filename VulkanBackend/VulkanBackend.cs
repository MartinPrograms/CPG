using CPG.Common;
using CPG.Interface;
using CPG.Interface.Settings;
using OpenGLBackend;
using Silk.NET.Windowing;
using IWindow = CPG.Interface.IWindow;
using OperatingSystem = CPG.Common.OperatingSystem;

namespace VulkanBackend;

public class VulkanBackend : IGraphicsInterface
{
    public string Name { get; } = "Vulkan";
    public GraphicsSystem SupportedSystem { get; }
    public bool IsSupported() => SupportedSystem.IsSupported();

    public bool MultiThreaded { get; }
    public bool MultiWindow { get; }
    public SettingsContainer Settings { get; set; } = new();
    public IWindow Create(WindowSettings settings)
    {
        return new WindowVK(settings, Settings);
    }
    
    public VulkanBackend()
    {
        SupportedSystem = new GraphicsSystem(OperatingSystem.Windows | OperatingSystem.MacOS | OperatingSystem.Linux);
        MultiThreaded = true;
        MultiWindow = false;
    }
}