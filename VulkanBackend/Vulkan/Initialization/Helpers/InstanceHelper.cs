using CPG.Common;
using OpenGLBackend;
using Silk.NET.Vulkan;
using VulkanBackend.Settings;

namespace VulkanBackend.Vulkan.Initialization.Helpers;

public class InstanceHelper
{
    public unsafe static Instance CreateInstance(InitializationSettings settings, WindowVK window,
        out DebugUtilsMessengerEXT messengerExt)
    {
        var appCreateInfo = new ApplicationInfo
        {
            SType = StructureType.ApplicationInfo,
            PApplicationName = settings.ApplicationName.ToPointer(),
            ApplicationVersion = settings.ApplicationVersion,
            PEngineName = settings.EngineName.ToPointer(),
            EngineVersion = settings.EngineVersion,
            ApiVersion = settings.ApiVersion
        };
        
        List<string> layers = new();
        List<string> extensions = new();
        
        if (settings.ValidationLayers)
        {
            layers.Add("VK_LAYER_KHRONOS_validation");
        }

        if (settings.Debug)
        {
            extensions.Add("VK_EXT_debug_utils");
        }
        
        var windowExtensions = window.GetRequiredExtensions();
        
        var instanceCreateInfo = new InstanceCreateInfo
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appCreateInfo,
        };
    }
}