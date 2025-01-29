using Silk.NET.Vulkan;

namespace VulkanBackend.Vulkan.Rendering.Common;

public interface IRenderable
{
    void Apply(CommandBuffer commandBuffer, StateManager stateManager);
}