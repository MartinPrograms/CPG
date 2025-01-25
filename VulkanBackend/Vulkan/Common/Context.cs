using Silk.NET.Vulkan;

namespace VulkanBackend.Vulkan.Common;

/// <summary>
/// Context holding most of the Vulkan objects.
/// </summary>
public class Context
{
    public static Context Current { get; private set; }

    public Context()
    {
        Current = this;
    }
    
    public Vk Vk;
    public Instance Instance = default;
    public PhysicalDevice PhysicalDevice = default;
    public QueueFamilyIndices Indices = default;
    public Device Device = default;
    public Queue GraphicsQueue = default;
    public Queue PresentQueue = default;
    public SurfaceKHR Surface = default;
    public CommandPool CommandPool = default;

    public Stack<Action<Context>> DeletionQueue = new(); // Fifo, so we can delete things in the correct order.
}