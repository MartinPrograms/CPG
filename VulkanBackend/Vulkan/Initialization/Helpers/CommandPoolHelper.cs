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
        
        ClearValue colorClear = new()
        {
            Color = new ClearColorValue(0.2f, 0.3f, 0.5f, 1.0f),
        };
        
        ClearValue depthClear = new()
        {
            DepthStencil = new ClearDepthStencilValue(1.0f, 0)
        };
        
        ClearValue[] clearValues = new ClearValue[]
        {
            colorClear,
            depthClear
        };
        
        RenderPassBeginInfo renderPassInfo = new()
        {
            SType = StructureType.RenderPassBeginInfo,
            RenderPass = swapchain.RenderPass,
            Framebuffer = swapchain.Framebuffers[swapchain.CurrentImage],
            RenderArea = new Rect2D
            {
                Offset = new Offset2D(0, 0),
                Extent = swapchain.SwapchainExtent  
            },
            ClearValueCount = (uint)clearValues.Length,
            PClearValues = (ClearValue*)Unsafe.AsPointer(ref clearValues[0]),
        };
        
        vk.BeginCommandBuffer(commandBuffer, &beginInfo).ThrowCode();
        
        // Transition into color attachment
        var old = swapchain.ImageFormats[swapchain.CurrentImage];
        ImageLayoutHelper.TransitionImageLayout(commandBuffer, swapchain.SwapchainImages[swapchain.CurrentImage], swapchain.SwapchainMode.SwapchainImageFormat, old, ImageLayout.ColorAttachmentOptimal);
        swapchain.ImageFormats[swapchain.CurrentImage] = ImageLayout.ColorAttachmentOptimal;
        
        vk.CmdBeginRenderPass(commandBuffer, &renderPassInfo, SubpassContents.Inline);
        
        var viewport = new Viewport
        {
            X = 0,
            Y = 0,
            Width = swapchain.SwapchainExtent.Width,
            Height = swapchain.SwapchainExtent.Height,
            MinDepth = 0,
            MaxDepth = 1
        };
        
        var scissor = new Rect2D
        {
            Offset = new Offset2D(0, 0),
            Extent = swapchain.SwapchainExtent
        };
        
        vk.CmdSetViewport(commandBuffer, 0, 1, &viewport);
        vk.CmdSetScissor(commandBuffer, 0, 1, &scissor);
        

    }

    public static void EndCommandBuffer(CommandBuffer commandBuffer, CpgSwapchain swapchain)
    {
        var vk = Context.Current.Vk;
        vk.CmdEndRenderPass(commandBuffer);
        
        vk.EndCommandBuffer(commandBuffer).ThrowCode();
    }

    public static void ResetCommandBuffer(CommandBuffer commandBuffer)
    {
        var vk = Context.Current.Vk;
        
        vk.ResetCommandBuffer(commandBuffer, CommandBufferResetFlags.ReleaseResourcesBit).ThrowCode();
    }
}