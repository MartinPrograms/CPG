using System.Numerics;
using CPG;
using CPG.Common.Rendering;
using Silk.NET.Vulkan;
using VulkanBackend.Vulkan.Common;
using VulkanBackend.Vulkan.Initialization.Helpers;
using VulkanBackend.Vulkan.Rendering.Common;
using Viewport = VulkanBackend.Vulkan.Rendering.Common.Viewport;

namespace VulkanBackend.Vulkan.Rendering;

public class StateManager
{
    // Yes, this is not how Vulkan should be used, but this graphics api is meant to be similar to OpenGL, which is a state machine.
    public List<IRenderable> Renderables { get; set; } = new();
    private IRenderable? _recentRenderPass = default;
    
    public Framebuffer CurrentFramebuffer { get; set; }
    public RenderPass CurrentRenderPass { get; set; }
    public Pipeline CurrentPipeline { get; set; }
    public DescriptorSet CurrentDescriptorSet { get; set; }
    
    public Vector2 FramebufferSize { get; set; }
    
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
        
        CurrentFramebuffer = swapchain.Framebuffers[index];
        FramebufferSize = new Vector2(swapchain.SwapchainExtent.Width, swapchain.SwapchainExtent.Height);
        CurrentRenderPass = swapchain.RenderPass;
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
        
        // Call all the renderables, from last to first
        for (int i = Renderables.Count - 1; i >= 0; i--)
        {
            var item = Renderables[i];
            if (_recentRenderPass != item && _recentRenderPass is Clear && item is Clear)
            {
                // If the last item was a clear, we need to end the render pass, otherwise we run 2 render passes at once
                vk.CmdEndRenderPass(currentCommandBuffer);
                _recentRenderPass = item;
            }
            
            item.Apply(currentCommandBuffer, this);
        }
        
        Renderables.Clear();
        
        vk.CmdEndRenderPass(commandBuffer);
        
        
        CommandPoolHelper.EndCommandBuffer(commandBuffer);
        
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

    public void Clear(ClearMask mask)
    {
        Renderables.Add(new Clear(){Mask = mask});
    }

    public void SetClearColor(Vector4 color)
    {
        // Get the last item in the renderables list, if it is a clear, set the color
        if (Renderables.Count > 0)
        {
            if (Renderables[^1] is Clear clear)
            {
                clear.Color = color;
            }
        }
    }

    public void SetViewport(int i, int i1, int width, int height) // Same thing as renderpass
    {
        Renderables.Add(new Viewport(){X = i, Y = i1, Width = width, Height = height});
    }
}