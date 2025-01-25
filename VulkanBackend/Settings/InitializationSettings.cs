using CPG.Interface;

namespace VulkanBackend.Settings;

public class InitializationSettings : ISetting
{
    public string Name { get; } = "Initialization";
    public bool Debug { get; set; } = false;
    public bool ValidationLayers { get; set; } = false;
    
    public List<string> EnabledLayers { get; set; } = new();
    public List<string> EnabledExtensions { get; set; } = new();
}