using Microsoft.VisualBasic;
using Silk.NET.Vulkan;
using VulkanBackend.Vulkan.Common;

namespace VulkanBackend.Vulkan.Rendering.Helpers;

public class ImageLayoutHelper
{
    public static unsafe void TransitionImageLayout(CommandBuffer contextCommandBuffer, Image swapchainSwapchainImage, Format swapchainModeSwapchainImageFormat, ImageLayout old, ImageLayout @new)
    {
        if (old == @new)
        {
            return;
        }
        
        var vk = Context.Current?.Vk;
        if (vk == null)
        {
            throw new Exception("Vulkan API is not initialized");
        }

        ImageMemoryBarrier barrier = new()
        {
            SType = StructureType.ImageMemoryBarrier,
            OldLayout = old,
            NewLayout = @new,
            SrcQueueFamilyIndex = Vk.QueueFamilyIgnored,
            DstQueueFamilyIndex = Vk.QueueFamilyIgnored,
            Image = swapchainSwapchainImage,
            SubresourceRange = new ImageSubresourceRange
            {
                AspectMask = ImageAspectFlags.ColorBit,
                BaseMipLevel = 0,
                LevelCount = 1,
                BaseArrayLayer = 0,
                LayerCount = 1
            }
        };

        AccessFlags sourceAccessMask;
        AccessFlags destinationAccessMask;

        if (old == ImageLayout.Undefined && @new == ImageLayout.ColorAttachmentOptimal)
        {
            barrier.SrcAccessMask = 0;
            barrier.DstAccessMask = AccessFlags.ColorAttachmentWriteBit;

            sourceAccessMask = 0;
            destinationAccessMask = AccessFlags.ColorAttachmentWriteBit;
        }
        else if (old == ImageLayout.ColorAttachmentOptimal && @new == ImageLayout.PresentSrcKhr)
        {
            barrier.SrcAccessMask = AccessFlags.ColorAttachmentWriteBit;
            barrier.DstAccessMask = AccessFlags.MemoryReadBit;

            sourceAccessMask = AccessFlags.ColorAttachmentWriteBit;
            destinationAccessMask = AccessFlags.MemoryReadBit;
        }
        else if (old == ImageLayout.Undefined && @new == ImageLayout.PresentSrcKhr)
        {
            barrier.SrcAccessMask = 0;
            barrier.DstAccessMask = AccessFlags.MemoryReadBit;

            sourceAccessMask = 0;
            destinationAccessMask = AccessFlags.MemoryReadBit;
        }
        else
        {
            throw new Exception($"Unsupported layout transition combination {old} to {@new}");
        }

        vk.CmdPipelineBarrier(contextCommandBuffer, PipelineStageFlags.ColorAttachmentOutputBit, PipelineStageFlags.ColorAttachmentOutputBit, 0, 0, null, 0, null, 1, &barrier);
    }
}