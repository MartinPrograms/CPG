using CPG.Common;
using Silk.NET.Vulkan;
using VulkanBackend.Settings;
using VulkanBackend.Vulkan.Common;

namespace VulkanBackend.Vulkan.Initialization.Helpers;

public static class LogicalDeviceHelper
{
    public static unsafe Device CreateLogicalDevice(PhysicalDevice physicalDevice, QueueFamilyIndices indices, InitializationSettings settings)
    {
        var vk = Context.Current.Vk;
        float priority = 1.0f;
        var queueCreateInfo = new DeviceQueueCreateInfo
        {
            SType = StructureType.DeviceQueueCreateInfo,
            QueueFamilyIndex = indices.GraphicsFamily,
            QueueCount = 1,
            PQueuePriorities = &priority
        };

        var deviceFeatures = new PhysicalDeviceFeatures
        {
            GeometryShader = true
        };
        
        var extensions = settings.EnabledDeviceExtensions;

        var createInfo = new DeviceCreateInfo
        {
            SType = StructureType.DeviceCreateInfo,
            QueueCreateInfoCount = 1,
            PQueueCreateInfos = &queueCreateInfo,
            EnabledExtensionCount = (uint) extensions.Count,
            PpEnabledExtensionNames = extensions.ToPointerArray(),
            PEnabledFeatures = &deviceFeatures
        };

        Device logicalDevice;
        if (vk.CreateDevice(physicalDevice, &createInfo, null, &logicalDevice) != Result.Success)
        {
            throw new System.Exception("Failed to create logical device!");
        }

        return logicalDevice;
    }

    public static Queue GetGraphicsQueue(Device logicalDevice, QueueFamilyIndices queueFamilyIndices)
    {
        var vk = Context.Current.Vk;
        vk.GetDeviceQueue(logicalDevice, queueFamilyIndices.GraphicsFamily, 0, out var queue);
        return queue;
    }
}