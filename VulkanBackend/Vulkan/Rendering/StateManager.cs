using CPG;
using Silk.NET.Vulkan;
using VulkanBackend.Vulkan.Common;
using VulkanBackend.Vulkan.Initialization.Helpers;

namespace VulkanBackend.Vulkan.Rendering;

public class StateManager
{
    // Yes, this is not how Vulkan should be used, but this graphics api is meant to be similar to OpenGL, which is a state machine.
    
    private CommandBuffer currentCommandBuffer = default;
    private bool _frameStarted = false;
    private bool _recreateSwapchain = false;
    public unsafe void BeginFrame()
    {
        if (_frameStarted)
        {
            Logger.Warning("Frame already started", "VulkanBackend");
            return;
        }
        
        var vk = Context.Current.Vk;
        var swapchain = Context.Current.Swapchain;
        uint currentFrame = swapchain.CurrentImage % swapchain.ImageCount;
        swapchain.CurrentImage = currentFrame;

        if (_recreateSwapchain)
        {
            Logger.Warning("Recreating swapchain", "VulkanBackend");
            _recreateSwapchain = false;
         
            SwapchainHelper.RecreateSwapchain(swapchain);
            
            swapchain.CurrentImage = 0;
            currentFrame = 0;

            _frameStarted = false;
            return;
        }
        
        var imageAvailableSemaphore = swapchain.ImageAvailableSemaphores[currentFrame];
        var renderFinishedSemaphore = swapchain.RenderFinishedSemaphores[currentFrame];
        var inFlightFence = swapchain.InFlightFences[currentFrame];
        
        vk.WaitForFences(Context.Current.Device, 1u, new[] { inFlightFence }, true, ulong.MaxValue);
        vk.ResetFences(Context.Current.Device, 1u, new[] { inFlightFence });

        uint index = 0;
        var acquireResult = swapchain.SwapchainExtension.AcquireNextImage(Context.Current.Device, swapchain.Swapchain, ulong.MaxValue, imageAvailableSemaphore, default, &index);

        if (acquireResult == Result.ErrorOutOfDateKhr || acquireResult == Result.SuboptimalKhr)
        {
            _recreateSwapchain = true;
            Logger.Warning("Swapchain out of date or suboptimal (BeginFrame)", "VulkanBackend");
            _frameStarted = false;
            return;
        }
        else if (acquireResult != Result.Success)
        {
            throw new System.Exception($"Failed to acquire next image: {acquireResult}");
        }
        
        var commandBuffer = swapchain.CommandBuffers[index];
        currentCommandBuffer = commandBuffer;
        CommandPoolHelper.ResetCommandBuffer(commandBuffer);
        CommandPoolHelper.BeginCommandBuffer(commandBuffer, CommandBufferUsageFlags.OneTimeSubmitBit, swapchain);
        
        // Now the user can start calling Graphics API functions, which will be translated into Vulkan commands
        _frameStarted = true;
    }

    public unsafe void EndFrame()
    {
        if (_frameStarted == false)
        {
            Logger.Warning("Frame not started", "VulkanBackend");
            return;
        }
        
        var vk = Context.Current.Vk;
        var swapchain = Context.Current.Swapchain;

        var waitDstStageMask = PipelineStageFlags.ColorAttachmentOutputBit;
        
        var imageAvailableSemaphore = swapchain.ImageAvailableSemaphores[swapchain.CurrentImage];
        var renderFinishedSemaphore = swapchain.RenderFinishedSemaphores[swapchain.CurrentImage];
        var inFlightFence = swapchain.InFlightFences[swapchain.CurrentImage];
        var commandBuffer = swapchain.CommandBuffers[swapchain.CurrentImage];
        
        CommandPoolHelper.EndCommandBuffer(commandBuffer, swapchain);
        
        var submitInfo = new SubmitInfo
        {
            SType = StructureType.SubmitInfo,
            WaitSemaphoreCount = 1,
            PWaitSemaphores = &imageAvailableSemaphore,
            PWaitDstStageMask = &waitDstStageMask,
            CommandBufferCount = 1,
            PCommandBuffers = &commandBuffer,
            SignalSemaphoreCount = 1,
            PSignalSemaphores = &renderFinishedSemaphore
        };
        
        vk.QueueSubmit(Context.Current.GraphicsQueue, 1, &submitInfo, inFlightFence);
        
        // Present
        var swapchainKhr = swapchain.Swapchain;
        var currentImage = swapchain.CurrentImage;
        var presentInfo = new PresentInfoKHR
        {
            SType = StructureType.PresentInfoKhr,
            WaitSemaphoreCount = 1,
            PWaitSemaphores = &renderFinishedSemaphore,
            SwapchainCount = 1,
            PSwapchains = &swapchainKhr,
            PImageIndices = &currentImage
        };
        
        var presentResult = swapchain.SwapchainExtension.QueuePresent(Context.Current.PresentQueue, &presentInfo);
        
        if (presentResult == Result.ErrorOutOfDateKhr || presentResult == Result.SuboptimalKhr)
        {
            Logger.Warning("Swapchain out of date or suboptimal (EndFrame)", "VulkanBackend");
            _recreateSwapchain = true;
            swapchain.CurrentImage = 0;
        }
        else if (presentResult != Result.Success)
        {
            throw new System.Exception($"Failed to present image: {presentResult}");
        }
        
        swapchain.CurrentImage++;
        
        _frameStarted = false;
    }
}