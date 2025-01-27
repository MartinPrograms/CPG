using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using VulkanBackend.Vulkan.Common;

namespace VulkanBackend.Vulkan.Initialization.Helpers;

public class SurfaceHelper
{
    public static SurfaceKHR CreateSurface(Instance instance, WindowVK window)
    {
        return window.CreateSurface(instance);
    }

    public static unsafe void DestroySurface(Instance objInstance, SurfaceKHR objSurface)
    {
        Context.Current.Vk.TryGetInstanceExtension<KhrSurface>(objInstance, out var ext);
        
        ext.DestroySurface(objInstance, objSurface, null);
    }
}