using System.Runtime.CompilerServices;
using Silk.NET.Vulkan;
using VulkanBackend.Vulkan.Common;
using VulkanBackend.Vulkan.Extensions;
using VulkanBackend.Vulkan.Rendering.Helpers;

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

    public static unsafe CommandBuffer CreateCommandBuffer(Device logicalDevice, CommandPool commandPool, CommandBufferLevel primary)
    {
        var vk = Context.Current.Vk;
        
        CommandBufferAllocateInfo allocInfo = new()
        {
            SType = StructureType.CommandBufferAllocateInfo,
            CommandPool = commandPool,
            Level = primary,
            CommandBufferCount = 1
        };
        
        vk.AllocateCommandBuffers(logicalDevice, &allocInfo, out CommandBuffer commandBuffer).ThrowCode();
        
        return commandBuffer;
    }

    public static unsafe void BeginCommandBuffer(CommandBuffer commandBuffer, CommandBufferUsageFlags simultaneousUseBit, CpgSwapchain swapchain)
    {
        var vk = Context.Current.Vk;
        
        CommandBufferBeginInfo beginInfo = new()
        {
            SType = StructureType.CommandBufferBeginInfo,
            Flags = simultaneousUseBit
        };
        
        vk.BeginCommandBuffer(commandBuffer, &beginInfo).ThrowCode();
        
        // Transition into color attachment
        var old = swapchain.ImageFormats[swapchain.CurrentImage];
        ImageLayoutHelper.TransitionImageLayout(commandBuffer, swapchain.SwapchainImages[swapchain.CurrentImage], swapchain.SwapchainMode.SwapchainImageFormat, old, ImageLayout.ColorAttachmentOptimal);
        swapchain.ImageFormats[swapchain.CurrentImage] = ImageLayout.ColorAttachmentOptimal;
        
    }

    public static void EndCommandBuffer(CommandBuffer commandBuffer)
    {
        var vk = Context.Current.Vk;
        
        vk.EndCommandBuffer(commandBuffer).ThrowCode();
    }

    public static void ResetCommandBuffer(CommandBuffer commandBuffer)
    {
        var vk = Context.Current.Vk;
        
        vk.ResetCommandBuffer(commandBuffer, CommandBufferResetFlags.ReleaseResourcesBit).ThrowCode();
    }
}