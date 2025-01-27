using Silk.NET.Vulkan;
using VulkanBackend.Vulkan.Common;

namespace VulkanBackend.Vulkan.Initialization.Helpers;

public static class ImageHelper
{
    public static unsafe void CreateImage(uint swapchainExtentWidth, uint swapchainExtentHeight, Format depthFormat, ImageTiling optimal, ImageUsageFlags depthStencilAttachmentBit, MemoryPropertyFlags deviceLocalBit, out Image depthImage, out DeviceMemory depthImageMemory)
    {
        var vk = Context.Current.Vk;
        if (vk == null)
        {
            throw new Exception("Vulkan API is not initialized");
        }

        ImageCreateInfo imageInfo = new()
        {
            SType = StructureType.ImageCreateInfo,
            ImageType = ImageType.Type2D,
            Extent = new Extent3D
            {
                Width = swapchainExtentWidth,
                Height = swapchainExtentHeight,
                Depth = 1
            },
            MipLevels = 1,
            ArrayLayers = 1,
            Format = depthFormat,
            Tiling = optimal,
            InitialLayout = ImageLayout.Undefined,
            Usage = depthStencilAttachmentBit,
            Samples = SampleCountFlags.Count1Bit,
            SharingMode = SharingMode.Exclusive
        };

        vk.CreateImage(Context.Current.Device, &imageInfo, null, out depthImage);

        MemoryRequirements memoryRequirements;
        vk.GetImageMemoryRequirements(Context.Current.Device, depthImage, &memoryRequirements);

        MemoryAllocateInfo allocateInfo = new()
        {
            SType = StructureType.MemoryAllocateInfo,
            AllocationSize = memoryRequirements.Size,
            MemoryTypeIndex = FindMemoryType(memoryRequirements.MemoryTypeBits, deviceLocalBit)
        };

        vk.AllocateMemory(Context.Current.Device, &allocateInfo, null, out depthImageMemory);
        vk.BindImageMemory(Context.Current.Device, depthImage, depthImageMemory, 0);
    }
    public static unsafe ImageView CreateImageView(Image depthImage, Format depthFormat, ImageAspectFlags imageAspectDepthBit)
    {
        var vk = Context.Current?.Vk;
        if (vk == null)
        {
            throw new Exception("Vulkan API is not initialized");
        }

        ImageViewCreateInfo viewInfo = new()
        {
            SType = StructureType.ImageViewCreateInfo,
            Image = depthImage,
            ViewType = ImageViewType.Type2D,
            Format = depthFormat,
            SubresourceRange = new ImageSubresourceRange
            {
                AspectMask = imageAspectDepthBit,
                BaseMipLevel = 0,
                LevelCount = 1,
                BaseArrayLayer = 0,
                LayerCount = 1
            }
        };

        ImageView imageView;
        vk.CreateImageView(Context.Current!.Device, &viewInfo, null, &imageView);
        return imageView;
    }

    public static uint FindMemoryType(uint memoryRequirementsMemoryTypeBits, MemoryPropertyFlags flags)
    {
        var vk = Context.Current.Vk;
        if (vk == null)
        {
            throw new Exception("Vulkan API is not initialized");
        }
        
        var memoryProperties = vk.GetPhysicalDeviceMemoryProperties(Context.Current.PhysicalDevice);
        
        for (uint i = 0; i < memoryProperties.MemoryTypeCount; i++)
        {
            if ((memoryRequirementsMemoryTypeBits & (1 << (int)i)) != 0 && (memoryProperties.MemoryTypes[(int)i].PropertyFlags & flags) == flags)
            {
                return i;
            }
        }
        
        throw new Exception("Failed to find suitable memory type");
    }
}