using Silk.NET.Vulkan;
using VulkanBackend.Vulkan.Common;

namespace VulkanBackend.Vulkan.Rendering.Common;

public class Viewport : IRenderable
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public unsafe void Apply(CommandBuffer commandBuffer, StateManager stateManager)
    {
        var vk = Context.Current.Vk;
        Silk.NET.Vulkan.Viewport viewport = new()
        {
            X = X,
            Y = Y,
            Width = Width,
            Height = Height,
            MinDepth = 0,
            MaxDepth = 1
        };
        
        Rect2D scissor = new()
        {
            Offset = new Offset2D((int)X, (int)Y),
            Extent = new Extent2D((uint)Width, (uint)Height)
        };
        
        vk.CmdSetViewport(commandBuffer, 0, 1, &viewport);
        vk.CmdSetScissor(commandBuffer, 0, 1, &scissor);
    }
}