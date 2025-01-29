using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace VulkanBackend.Vulkan.Common;

/// <summary>
/// Objects that share a name with Vulkan objects are prefixed with Cpg to avoid conflicts.
/// </summary>
public class CpgSwapchain
{
    public SwapchainKHR Swapchain = default;
    public Image[] SwapchainImages = default;
    public ImageView[] SwapchainImageViews = default;
    public Extent2D SwapchainExtent = default;
    public SwapchainMode SwapchainMode = default;
    public ImageLayout[] ImageFormats = default;
    public uint ImageCount = default;
    public uint CurrentImage = default;
    public Semaphore[] ImageAvailableSemaphores = default;
    public Semaphore[] RenderFinishedSemaphores = default;
    public Fence[] InFlightFences = default;
    public Fence[] ImagesInFlight = default;
    public RenderPass RenderPass = default;
    public Framebuffer[] Framebuffers = default;
    public CommandBuffer[] CommandBuffers = default;
    public Image[] DepthImages = default;
    public ImageView[] DepthImageViews = default;
    public DeviceMemory[] DepthImageMemory = default;
    public KhrSwapchain SwapchainExtension = default;
}