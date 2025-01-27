using Silk.NET.Vulkan;
using VulkanBackend.Vulkan.Common;

namespace VulkanBackend.Vulkan.Initialization.Helpers;

public static class QueueFamilyHelper
{
    public static unsafe QueueFamilyIndices GetQueueFamilyIndices(PhysicalDevice physicalDevice, SurfaceKHR surface)
    {
        var vk = Context.Current.Vk;
        QueueFamilyIndices indices = new();

        uint queueFamilyCount = 0;
        vk.GetPhysicalDeviceQueueFamilyProperties(physicalDevice, &queueFamilyCount, null);

        QueueFamilyProperties* queueFamilies = stackalloc QueueFamilyProperties[(int)queueFamilyCount];
        vk.GetPhysicalDeviceQueueFamilyProperties(physicalDevice, &queueFamilyCount, queueFamilies);

        for (uint i = 0; i < queueFamilyCount; i++)
        {
            if (queueFamilies[i].QueueFlags.HasFlag(QueueFlags.GraphicsBit))
            {
                indices.GraphicsFamily = i;
            }

            if (indices.IsComplete)
            {
                break;
            }
        }

        return indices;

    }
}

public class QueueFamilyIndices
{
    public uint GraphicsFamily { get; set; } = uint.MaxValue;
    public bool IsComplete => GraphicsFamily != uint.MaxValue;

}