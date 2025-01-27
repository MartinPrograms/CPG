using Silk.NET.Vulkan;

namespace VulkanBackend.Vulkan.Initialization.Helpers;

public class SurfaceHelper
{
    public static SurfaceKHR CreateSurface(Instance instance, WindowVK window)
    {
        return window.CreateSurface(instance);
    }
}