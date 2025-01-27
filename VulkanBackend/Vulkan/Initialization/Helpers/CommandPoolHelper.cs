using Silk.NET.Vulkan;
using VulkanBackend.Vulkan.Common;
using VulkanBackend.Vulkan.Extensions;

namespace VulkanBackend.Vulkan.Initialization.Helpers;

public static class CommandPoolHelper
{
    public static unsafe CommandPool CreateCommandPool(Device logicalDevice, QueueFamilyIndices queueFamilyIndices)
    {
        var vk = Context.Current.Vk;
        
        CommandPoolCreateInfo poolInfo = new()
        {
            SType = StructureType.CommandPoolCreateInfo,
            QueueFamilyIndex = queueFamilyIndices.GraphicsFamily,
            Flags = CommandPoolCreateFlags.ResetCommandBufferBit
        };
        
        vk.CreateCommandPool(logicalDevice, &poolInfo, null, out CommandPool commandPool).ThrowCode();
        
        return commandPool;
    }
}