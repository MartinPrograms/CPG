using Silk.NET.Vulkan;

namespace VulkanBackend.Vulkan.Common;

public class QueueFamilyIndices 
{
    public uint GraphicsFamily { get; set; } = uint.MaxValue;
    public bool IsComplete => GraphicsFamily != uint.MaxValue;

    public static unsafe QueueFamilyIndices FindQueueFamilies(PhysicalDevice physicalDevice)
    {
        var vk = Context.Current.Vk;
        QueueFamilyIndices indices = new();

        uint queueFamilyCount = 0;
        vk.GetPhysicalDeviceQueueFamilyProperties(physicalDevice,&queueFamilyCount, null);

        QueueFamilyProperties* queueFamilies = stackalloc QueueFamilyProperties[(int)queueFamilyCount];
        vk.GetPhysicalDeviceQueueFamilyProperties(physicalDevice,&queueFamilyCount, queueFamilies);

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