using CPG.Interface;
using VulkanBackend.Vulkan.Common;

namespace VulkanBackend.Settings;

public class InitializationSettings : ISetting
{
    public string Name { get; } = "Initialization";
    public bool Debug { get; set; } = false;
    public bool ValidationLayers { get; set; } = false;
    
    public List<string> EnabledLayers { get; set; } = new();
    public List<string> EnabledExtensions { get; set; } = new();
    public List<string> EnabledDeviceExtensions { get; set; } = new();
    
    public string ApplicationName { get; set; } = "CPG Application";
    public string EngineName { get; set; } = "CPG Engine";
    public VulkanVersion ApplicationVersion { get; set; } = new(1,0,0);
    public VulkanVersion EngineVersion { get; set; } = new(1,0,0);
    public VulkanVersion ApiVersion { get; set; } = new(1,0,0);
}