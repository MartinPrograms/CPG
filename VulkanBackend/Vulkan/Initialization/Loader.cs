using VulkanBackend.Settings;
using VulkanBackend.Vulkan.Common;
using VulkanBackend.Vulkan.Initialization.Helpers;

namespace VulkanBackend.Vulkan.Initialization;

public class Loader
{
    public static void LoadVulkan(ref Context context, InitializationSettings settings, WindowVK window)
    {
        context.Settings = settings;
        
        var instance = InstanceHelper.CreateInstance(settings, window, out var debugUtils);
        context.Instance = instance;
        context.DebugUtils = debugUtils;
        
        var surface = SurfaceHelper.CreateSurface(instance, window);
        context.Surface = surface;
        
        var physicalDevice = PhysicalDeviceHelper.GetPhysicalDevice(instance, surface);
        context.PhysicalDevice = physicalDevice;
        
        var queueFamilyIndices = QueueFamilyHelper.GetQueueFamilyIndices(physicalDevice, surface);
        context.QueueFamilyIndices = queueFamilyIndices;

        var logicalDevice = LogicalDeviceHelper.CreateLogicalDevice(physicalDevice, queueFamilyIndices, settings);
        context.Device = logicalDevice;
        
        var graphicsQueue = LogicalDeviceHelper.GetGraphicsQueue(logicalDevice, queueFamilyIndices);
        context.GraphicsQueue = graphicsQueue;
        
        var commandPool = CommandPoolHelper.CreateCommandPool(logicalDevice, queueFamilyIndices);
        context.CommandPool = commandPool;
        
        var cpgSwapchain = SwapchainHelper.CreateSwapchain(instance, physicalDevice, logicalDevice, surface, window, queueFamilyIndices);
        context.Swapchain = cpgSwapchain;
        
        // This concludes the default initialization of the Vulkan backend.
        // Now we can move on to creating pipelines, meshes and other stuff.
    }
}