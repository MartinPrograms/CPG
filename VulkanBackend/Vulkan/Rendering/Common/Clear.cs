using System.Numerics;
using System.Runtime.CompilerServices;
using CPG.Common.Rendering;
using Silk.NET.Vulkan;
using VulkanBackend.Vulkan.Common;

namespace VulkanBackend.Vulkan.Rendering.Common;

public class Clear : IRenderable
{
    public ClearMask Mask { get; set; }
    public Vector4 Color { get; set; }
    
    public unsafe void Apply(CommandBuffer commandBuffer, StateManager stateManager)
    {
        var vk = Context.Current.Vk;
        ClearValue clearValue = new()
        {
            Color = new ClearColorValue(Color.X, Color.Y, Color.Z, Color.W)
        };
        
        ClearValue depthClear = new()
        {
            DepthStencil = new ClearDepthStencilValue(1.0f, 0)
        };

        List<ClearValue> clearValues = new();
        
        if (Mask.HasFlag(ClearMask.Color))
        {
            clearValues.Add(clearValue);
        }
        
        if (Mask.HasFlag(ClearMask.Depth))
        {
            clearValues.Add(depthClear);
        }
        
        ClearValue[] clearValuesArray = clearValues.ToArray();
        
        // Create a new render pass
        
        RenderPassBeginInfo renderPassBeginInfo = new()
        {
            SType = StructureType.RenderPassBeginInfo,
            RenderPass = stateManager.CurrentRenderPass,
            Framebuffer = stateManager.CurrentFramebuffer,
            RenderArea = new Rect2D
            {
                Offset = new Offset2D(0, 0),
                Extent = new Extent2D((uint?)stateManager.FramebufferSize.X, (uint?)stateManager.FramebufferSize.Y)
            },
            ClearValueCount = (uint)clearValues.Count,
            PClearValues = (ClearValue*)Unsafe.AsPointer(ref clearValuesArray[0]),
        };
        
        vk.CmdBeginRenderPass(commandBuffer, &renderPassBeginInfo, SubpassContents.Inline);
        
        // Default scissors and viewports
        Silk.NET.Vulkan.Viewport viewport = new()
        {
            X = 0,
            Y = 0,
            Width = stateManager.FramebufferSize.X,
            Height = stateManager.FramebufferSize.Y,
            MinDepth = 0,
            MaxDepth = 1
        };
        
        Rect2D scissor = new()
        {
            Offset = new Offset2D(0, 0),
            Extent = new Extent2D((uint?)stateManager.FramebufferSize.X, (uint?)stateManager.FramebufferSize.Y)
        };
        
        vk.CmdSetViewport(commandBuffer, 0, 1, &viewport);
        vk.CmdSetScissor(commandBuffer, 0, 1, &scissor);
    }
}