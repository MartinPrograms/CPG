using CPG;
using VulkanBackend.Settings;
using VulkanBackend.Vulkan.Common;
using VulkanBackend.Vulkan.Initialization.Helpers;

namespace VulkanBackend.Vulkan.Initialization;

public class Loader
{
    public static void LoadVulkan(ref Context context, InitializationSettings settings, WindowVK window)
    {
        context.Settings = settings;
        context.Window = window;
        
        context.DeletionQueue.Push(c =>
        {
            unsafe
            {
                // Swapchain already added to deletion queue
                c.Vk.DestroyCommandPool(c.Device, c.CommandPool, null);
                SurfaceHelper.DestroySurface(c.Instance, c.Surface);
                c.Vk.DestroyDevice(c.Device, null);
                InstanceHelper.DestroyInstance(c.Instance, c.DebugUtils);
            }
        });
        
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
        
        var presentQueue = LogicalDeviceHelper.GetPresentQueue(logicalDevice, queueFamilyIndices);
        context.PresentQueue = presentQueue;
        
        var commandPool = CommandPoolHelper.CreateCommandPool(logicalDevice, queueFamilyIndices);
        context.CommandPool = commandPool;
        
        var cpgSwapchain = SwapchainHelper.CreateSwapchain(instance, physicalDevice, logicalDevice, surface, window, queueFamilyIndices);
        context.Swapchain = cpgSwapchain;
        
        // This concludes the default initialization of the Vulkan backend.
        // Now we can move on to creating pipelines, meshes and other stuff.
    }

    public static unsafe void Cleanup(ref Context context)
    {
        var stack = context.DeletionQueue;
        while (stack.Count > 0)
        {
            Logger.Info($"Cleaning up {stack.Count}", "VulkanBackend");
            stack.Pop().Invoke(context);
        }

        Logger.Info("Vulkan cleanup complete", "VulkanBackend");
        Logger.Info("Goodbye :3", "VulkanBackend");
    }
}