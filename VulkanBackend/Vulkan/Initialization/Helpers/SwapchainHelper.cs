using System.Runtime.CompilerServices;
using CPG;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using VulkanBackend.Vulkan.Common;
using VulkanBackend.Vulkan.Extensions;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace VulkanBackend.Vulkan.Initialization.Helpers;

public static class SwapchainHelper
{
    public static CpgSwapchain CreateSwapchain(Instance instance, PhysicalDevice physicalDevice, Device logicalDevice, SurfaceKHR surface, WindowVK window, QueueFamilyIndices queueFamilyIndices)
    {
        // Longest method in the project, most of it is pretty trivial though.
        // Create object, set properties, return object, times 200 lol...
        
        var cpgSwapchain = new CpgSwapchain();
        var vk = Context.Current.Vk;
        
        if (!vk.TryGetDeviceExtension<KhrSwapchain>(instance, logicalDevice, out var swapchainExtension))
        {
            throw new Exception("Failed to get swapchain extension");
        }

        cpgSwapchain.SwapchainExtension = swapchainExtension;
        
        DateTime startTime = DateTime.Now;
        
        Init(ref cpgSwapchain, surface, instance, physicalDevice, window);
        CreateSwapchainKHR(ref cpgSwapchain, logicalDevice, queueFamilyIndices, surface);
        CreateSwapchainImages(ref cpgSwapchain, logicalDevice);
        CreateSwapchainSemaphores(ref cpgSwapchain, logicalDevice);
        CreateSwapchainFences(ref cpgSwapchain, logicalDevice);
        CreateSwapchainDepthImage(ref cpgSwapchain, logicalDevice);
        CreateSwapchainRenderPass(ref cpgSwapchain, logicalDevice);
        CreateSwapchainFramebuffers(ref cpgSwapchain, logicalDevice);

        DateTime endTime = DateTime.Now;
        
        Context.Current.DeletionQueue.Push(c =>
        {
            DeleteSwapchain(c.Swapchain);
        });
        
        Logger.Info("Swapchain created in " + (endTime - startTime).TotalMilliseconds + "ms", "SwapchainHelper");
        // More info about the swapchain
        Logger.Info($"Swapchain Image Count: {cpgSwapchain.ImageCount}", "SwapchainHelper");
        Logger.Info($"Swapchain Image Format: {cpgSwapchain.SwapchainMode.SwapchainImageFormat}", "SwapchainHelper");
        Logger.Info($"Swapchain Color Space: {cpgSwapchain.SwapchainMode.SwapchainColorSpace}", "SwapchainHelper");
        Logger.Info($"Swapchain Present Mode: {cpgSwapchain.SwapchainMode.SwapchainPresentMode}", "SwapchainHelper");
        Logger.Info($"Swapchain Extent: {cpgSwapchain.SwapchainExtent.Width}x{cpgSwapchain.SwapchainExtent.Height}", "SwapchainHelper");
        
        return cpgSwapchain;
    }

    private static unsafe void DeleteSwapchain(CpgSwapchain objSwapchain)
    {
        var vk = Context.Current.Vk;
        var logicalDevice = Context.Current.Device;
        
        // Wait for the device to finish before deleting the swapchain
        vk.DeviceWaitIdle(logicalDevice);
        
        for (var i = 0; i < objSwapchain.ImageCount; i++)
        {
            vk.DestroyImageView(logicalDevice, objSwapchain.SwapchainImageViews[i], null);
            vk.DestroyImage(logicalDevice, objSwapchain.DepthImages[i], null);
            vk.FreeMemory(logicalDevice, objSwapchain.DepthImageMemory[i], null);
            vk.DestroyFramebuffer(logicalDevice, objSwapchain.Framebuffers[i], null);
            vk.DestroyFence(logicalDevice, objSwapchain.InFlightFences[i], null);
            vk.DestroyFence(logicalDevice, objSwapchain.ImagesInFlight[i], null);
            vk.DestroySemaphore(logicalDevice, objSwapchain.ImageAvailableSemaphores[i], null);
            vk.DestroySemaphore(logicalDevice, objSwapchain.RenderFinishedSemaphores[i], null);
        }
        
        vk.DestroyRenderPass(logicalDevice, objSwapchain.RenderPass, null);
        objSwapchain.SwapchainExtension.DestroySwapchain(logicalDevice, objSwapchain.Swapchain, null);
        
        Logger.Info("Swapchain deleted", "SwapchainHelper");
    }

    private static unsafe void CreateSwapchainFramebuffers(ref CpgSwapchain cpgSwapchain, Device logicalDevice)
    {
        var vk = Context.Current.Vk;
        cpgSwapchain.Framebuffers = new Framebuffer[cpgSwapchain.ImageCount];
        
        for (var i = 0; i < cpgSwapchain.ImageCount; i++)
        {
            var attachments = new ImageView[2] {cpgSwapchain.SwapchainImageViews[i], cpgSwapchain.DepthImageViews[i]};
            var framebufferCreateInfo = new FramebufferCreateInfo
            {
                SType = StructureType.FramebufferCreateInfo,
                RenderPass = cpgSwapchain.RenderPass,
                AttachmentCount = 2,
                PAttachments = (ImageView*)Unsafe.AsPointer(ref attachments[0]),
                Width = cpgSwapchain.SwapchainExtent.Width,
                Height = cpgSwapchain.SwapchainExtent.Height,
                Layers = 1
            };
            
            vk.CreateFramebuffer(logicalDevice, &framebufferCreateInfo, null, out cpgSwapchain.Framebuffers[i]).ThrowCode();
        }
    }

    private static unsafe void CreateSwapchainRenderPass(ref CpgSwapchain cpgSwapchain, Device logicalDevice)
    {
        var colorAttachment = new AttachmentDescription
        {
            Format = cpgSwapchain.SwapchainMode.SwapchainImageFormat,
            Samples = SampleCountFlags.Count1Bit,
            LoadOp = AttachmentLoadOp.Clear,
            StoreOp = AttachmentStoreOp.Store,
            StencilLoadOp = AttachmentLoadOp.DontCare,
            StencilStoreOp = AttachmentStoreOp.DontCare,
            InitialLayout = ImageLayout.Undefined,
            FinalLayout = ImageLayout.PresentSrcKhr
        };
        
        var depthAttachment = new AttachmentDescription
        {
            Format = Format.D32Sfloat,
            Samples = SampleCountFlags.Count1Bit,
            LoadOp = AttachmentLoadOp.Clear,
            StoreOp = AttachmentStoreOp.DontCare,
            StencilLoadOp = AttachmentLoadOp.DontCare,
            StencilStoreOp = AttachmentStoreOp.DontCare,
            InitialLayout = ImageLayout.Undefined,
            FinalLayout = ImageLayout.DepthStencilAttachmentOptimal
        };

        var colorAttachmentRef = new AttachmentReference
        {
            Attachment = 0,
            Layout = ImageLayout.ColorAttachmentOptimal
        };
        
        var depthAttachmentRef = new AttachmentReference
        {
            Attachment = 1,
            Layout = ImageLayout.DepthStencilAttachmentOptimal
        };

        var subpass = new SubpassDescription
        {
            PipelineBindPoint = PipelineBindPoint.Graphics,
            ColorAttachmentCount = 1,
            PColorAttachments = &colorAttachmentRef,
            PDepthStencilAttachment = &depthAttachmentRef
        };
        
        var dependency = new SubpassDependency
        {
            SrcSubpass = UInt32.MaxValue,
            DstSubpass = 0,
            SrcStageMask = PipelineStageFlags.ColorAttachmentOutputBit,
            SrcAccessMask = 0,
            DstStageMask = PipelineStageFlags.ColorAttachmentOutputBit,
            DstAccessMask = AccessFlags.ColorAttachmentReadBit | AccessFlags.ColorAttachmentWriteBit
        };
        
        var attachments = new AttachmentDescription[2] {colorAttachment, depthAttachment};
        var subpasses = new SubpassDescription[1] {subpass};
        var dependencies = new SubpassDependency[1] {dependency};
        
        var renderPassInfo = new RenderPassCreateInfo
        {
            SType = StructureType.RenderPassCreateInfo,
            AttachmentCount = (uint)attachments.Length,
            PAttachments = (AttachmentDescription*)Unsafe.AsPointer(ref attachments[0]),
            SubpassCount = (uint)subpasses.Length,
            PSubpasses = (SubpassDescription*)Unsafe.AsPointer(ref subpasses[0]),
            DependencyCount = (uint)dependencies.Length,
            PDependencies = (SubpassDependency*)Unsafe.AsPointer(ref dependencies[0])
        };
        
        var vk = Context.Current.Vk;
        vk.CreateRenderPass(logicalDevice, &renderPassInfo, null, out cpgSwapchain.RenderPass).ThrowCode();
    }

    private static void CreateSwapchainDepthImage(ref CpgSwapchain cpgSwapchain, Device logicalDevice)
    {
        var depthFormat = Format.D32Sfloat; // If this is not supported... get a new GPU.
        var vk = Context.Current.Vk;
        
        cpgSwapchain.DepthImages = new Image[cpgSwapchain.ImageCount];
        cpgSwapchain.DepthImageMemory = new DeviceMemory[cpgSwapchain.ImageCount];
        cpgSwapchain.DepthImageViews = new ImageView[cpgSwapchain.ImageCount];

        for (var i = 0; i < cpgSwapchain.ImageCount; i++)
        {
            ImageHelper.CreateImage(cpgSwapchain.SwapchainExtent.Width, cpgSwapchain.SwapchainExtent.Height, depthFormat, ImageTiling.Optimal, ImageUsageFlags.DepthStencilAttachmentBit, MemoryPropertyFlags.DeviceLocalBit, out cpgSwapchain.DepthImages[i], out cpgSwapchain.DepthImageMemory[i]);
            cpgSwapchain.DepthImageViews[i] = ImageHelper.CreateImageView(cpgSwapchain.DepthImages[i], depthFormat, ImageAspectFlags.DepthBit);

        }
    }

    private static unsafe void CreateSwapchainFences(ref CpgSwapchain cpgSwapchain, Device logicalDevice)
    {
        var vk = Context.Current.Vk;
        var fenceCreateInfo = new FenceCreateInfo
        {
            SType = StructureType.FenceCreateInfo,
            Flags = FenceCreateFlags.SignaledBit
        };
        
        cpgSwapchain.InFlightFences = new Fence[cpgSwapchain.ImageCount];
        cpgSwapchain.ImagesInFlight = new Fence[cpgSwapchain.ImageCount];
        
        for (var i = 0; i < cpgSwapchain.ImageCount; i++)
        {
            vk.CreateFence(logicalDevice, &fenceCreateInfo, null, out cpgSwapchain.InFlightFences[i]).ThrowCode();
            vk.CreateFence(logicalDevice, &fenceCreateInfo, null, out cpgSwapchain.ImagesInFlight[i]).ThrowCode();
        }
    }

    private static unsafe void CreateSwapchainSemaphores(ref CpgSwapchain cpgSwapchain, Device logicalDevice)
    {
        var vk = Context.Current.Vk;
        var semaphoreCreateInfo = new SemaphoreCreateInfo
        {
            SType = StructureType.SemaphoreCreateInfo,
            Flags = 0
        };
        
        cpgSwapchain.ImageAvailableSemaphores = new Semaphore[cpgSwapchain.ImageCount];
        cpgSwapchain.RenderFinishedSemaphores = new Semaphore[cpgSwapchain.ImageCount];
        
        for (var i = 0; i < cpgSwapchain.ImageCount; i++)
        { 
            vk.CreateSemaphore(logicalDevice, &semaphoreCreateInfo, null, out cpgSwapchain.ImageAvailableSemaphores[i]).ThrowCode();
            vk.CreateSemaphore(logicalDevice, &semaphoreCreateInfo, null, out cpgSwapchain.RenderFinishedSemaphores[i]).ThrowCode();
        }

    }

    /// <summary>
    /// Creates depth image & view for the swapchain.
    /// </summary>
    /// <param name="cpgSwapchain"></param>
    /// <param name="logicalDevice"></param>
    private static unsafe void CreateSwapchainImages(ref CpgSwapchain cpgSwapchain, Device logicalDevice)
    {
        var vk = Context.Current.Vk;
        var swapchainExtension = cpgSwapchain.SwapchainExtension;
        var Swapchain = cpgSwapchain.Swapchain;
        var SwapchainMode = cpgSwapchain.SwapchainMode;
        var ImageCount = cpgSwapchain.ImageCount;
        
        swapchainExtension.GetSwapchainImages(logicalDevice, Swapchain, &ImageCount, null);
        
        cpgSwapchain.SwapchainImages = new Image[ImageCount];
        Image* images = stackalloc Image[(int)ImageCount];

        swapchainExtension.GetSwapchainImages(logicalDevice, Swapchain, &ImageCount, images);
        

        for (var i = 0; i < ImageCount; i++)
        {
            cpgSwapchain.SwapchainImages[i] = images[i];
        }
        
        cpgSwapchain.SwapchainImageViews = new ImageView[ImageCount];
        for (var i = 0; i < ImageCount; i++)
        {
            var createInfoView = new ImageViewCreateInfo
            {
                SType = StructureType.ImageViewCreateInfo,
                Image = cpgSwapchain.SwapchainImages[i],
                ViewType = ImageViewType.Type2D,
                Format = SwapchainMode.SwapchainImageFormat,
                Components = new ComponentMapping
                {
                    R = ComponentSwizzle.R,
                    G = ComponentSwizzle.G,
                    B = ComponentSwizzle.B,
                    A = ComponentSwizzle.A
                },
                SubresourceRange = new ImageSubresourceRange
                {
                    AspectMask = ImageAspectFlags.ImageAspectColorBit,
                    BaseMipLevel = 0,
                    LevelCount = 1,
                    BaseArrayLayer = 0,
                    LayerCount = 1
                }
            };

            vk.CreateImageView(logicalDevice, &createInfoView, null, out cpgSwapchain.SwapchainImageViews[i])
                .ThrowCode();
        }
    }

    private static unsafe void CreateSwapchainKHR(ref CpgSwapchain cpgSwapchain, Device logicalDevice,
        QueueFamilyIndices queueFamilyIndices, SurfaceKHR surface)
    {
        var createInfo = new SwapchainCreateInfoKHR
        {
            SType = StructureType.SwapchainCreateInfoKhr,
            Surface = surface,
            MinImageCount = cpgSwapchain.ImageCount,
            ImageFormat = cpgSwapchain.SwapchainMode.SwapchainImageFormat,
            ImageColorSpace = cpgSwapchain.SwapchainMode.SwapchainColorSpace,
            ImageExtent = cpgSwapchain.SwapchainExtent,
            ImageArrayLayers = 1,
            ImageUsage = ImageUsageFlags.ColorAttachmentBit | ImageUsageFlags.TransferDstBit,
            ImageSharingMode = SharingMode.Exclusive,
            PreTransform = SurfaceTransformFlagsKHR.IdentityBitKhr,
            CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
            PresentMode = cpgSwapchain.SwapchainMode.SwapchainPresentMode,
            Clipped = true,
            OldSwapchain = default
        };

        cpgSwapchain.SwapchainExtension.CreateSwapchain(logicalDevice, &createInfo, null, out var swapchain).ThrowCode(); // Throws on error
        cpgSwapchain.Swapchain = swapchain;
    }

    private static void Init(ref CpgSwapchain cpgSwapchain, SurfaceKHR surface, Instance instance, PhysicalDevice physicalDevice,
        WindowVK window)
    {
        cpgSwapchain.ImageCount = 3; // One presentable image, one in flight, one in queue.
        cpgSwapchain.SwapchainMode = SwapchainHelper.GetSwapchainMode(physicalDevice, instance, surface);
        cpgSwapchain.SwapchainExtent = window.Extent();
    }

    private static unsafe SwapchainMode GetSwapchainMode(PhysicalDevice physicalDevice, Instance instance, SurfaceKHR surface)
    {
        var vk = Context.Current.Vk;
        
        Format format = Format.Undefined;
        ColorSpaceKHR colorSpace = ColorSpaceKHR.PaceSrgbNonlinearKhr;

        uint formatCount = 0;
        KhrSurface surfaceExtension;
        if (!vk.TryGetInstanceExtension<KhrSurface>(instance, out surfaceExtension))
        {
            throw new Exception("Failed to get surface extension");
        }
        
        surfaceExtension.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, &formatCount, null);
        if (formatCount == 0)
        {
            throw new Exception("Failed to get surface formats");
        }
        
        var formats = stackalloc SurfaceFormatKHR[(int)formatCount];
        surfaceExtension.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, &formatCount, formats);

        for (var i = 0; i < formatCount; i++)
        {
            var f = formats[i];
            if (f.Format == Format.B8G8R8A8Srgb && f.ColorSpace == ColorSpaceKHR.PaceSrgbNonlinearKhr)
            {
                format = f.Format;
                colorSpace = f.ColorSpace;
                break; // For now no HDR support
                
                // TODO: Add HDR support
            }
        }
        
        if (format == Format.Undefined)
        {
            format = formats[0].Format;
            colorSpace = formats[0].ColorSpace;
        }
        
        // Check if it supports mailbox mode, otherwise resort to FIFO
        uint presentModeCount = 0;
        surfaceExtension.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, &presentModeCount, null);
        if (presentModeCount == 0)
        {
            throw new Exception("Failed to get present modes");
        }
        
        var presentModes = stackalloc PresentModeKHR[(int)presentModeCount];
        surfaceExtension.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, &presentModeCount, presentModes);
        
        PresentModeKHR presentMode = PresentModeKHR.FifoKhr;
        for (var i = 0; i < presentModeCount; i++)
        {
            if (presentModes[i] == PresentModeKHR.MailboxKhr)
            {
                presentMode = presentModes[i];
                break;
            }
        }
        
        var a = new SwapchainMode(format, colorSpace);
        a.SwapchainPresentMode = presentMode;
        return a;
    }
}