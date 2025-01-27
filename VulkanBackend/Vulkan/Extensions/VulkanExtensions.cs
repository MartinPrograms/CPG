using Silk.NET.Vulkan;

namespace VulkanBackend.Vulkan.Extensions;

public static class VulkanExtensions
{
    public static void EnsureSuccess(this Vk vk, Result result)
    {
        if (result != Result.Success)
        {
            throw new System.Exception($"Vulkan Error: {result}");
        }
    }

    public static void ThrowCode(this Result result)
    {
        if (result != Result.Success)
        {
            throw new System.Exception($"Vulkan Error: {result}");
        }
    }
}