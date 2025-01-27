using CPG;
using CPG.Common;
using Silk.NET.Vulkan;
using VulkanBackend.Vulkan.Common;

namespace VulkanBackend.Vulkan.Initialization.Helpers;

public static class PhysicalDeviceHelper
{
    public unsafe static PhysicalDevice GetPhysicalDevice(Instance instance, SurfaceKHR surface)
    {
        var vk = Context.Current.Vk; // Guaranteed to be non-null

        uint count = 0;
        vk.EnumeratePhysicalDevices(instance, & count, null);
        
        var devices = stackalloc PhysicalDevice[(int) count];
        vk.EnumeratePhysicalDevices(instance, & count, devices); // annoying as hell, but whatever its a C API
        
        Logger.Info($"Found {count} physical devices", "PhysicalDeviceHelper");
        
        Dictionary<int, PhysicalDevice> suitableDevices = new();
        for (int i = 0; i < count; i++)
        {
            var device = devices[i];
            
            Logger.Info($"Checking device {i} {DeviceName(device)}", "PhysicalDeviceHelper");
            
            var suitability = IsDeviceSuitable(device, surface);
            
            if (suitability > 0)
            {
                suitableDevices.Add(suitability, device);
            }
        }
        
        if (suitableDevices.Count == 0)
        {
            throw new Exception("No suitable devices found");
        }
        
        var bestSuitability = suitableDevices.Keys.Max();
        return suitableDevices[bestSuitability];
    }
    
    private static unsafe string DeviceName(PhysicalDevice device)
    {
        var vk = Context.Current.Vk; // Guaranteed to be non-null
        var buffer = vk.GetPhysicalDeviceProperties(device);
        return UnsafeExtensions.ToString(buffer.DeviceName);
    }
    
    private static int IsDeviceSuitable(PhysicalDevice device, SurfaceKHR surface)
    {
        var vk = Context.Current.Vk; // Guaranteed to be non-null
        
        var properties = vk.GetPhysicalDeviceProperties(device);
        var features = vk.GetPhysicalDeviceFeatures(device);
        
        var settings = Context.Current.Settings;
        if (settings.IsDeviceSuitable == null)
        {
            // basic check:
            if (properties.DeviceType != PhysicalDeviceType.DiscreteGpu)
            {
                return 0;
            }

            // lord forgive me
            uint sum = 0;
            sum += properties.DeviceType == PhysicalDeviceType.DiscreteGpu ? 1000u : 0;
            sum += properties.Limits.MaxImageDimension2D;
            sum += properties.Limits.MaxViewports;
            sum += properties.Limits.MaxFramebufferWidth;
            sum += properties.Limits.MaxFramebufferHeight;
            sum += properties.Limits.MaxFramebufferLayers;
            sum += properties.Limits.MaxDescriptorSetUniformBuffers;
            sum += properties.Limits.MaxDescriptorSetStorageBuffers;
            sum += properties.Limits.MaxDescriptorSetSampledImages;
            sum += properties.Limits.MaxDescriptorSetSamplers;
            sum += properties.Limits.MaxDescriptorSetStorageImages;
            sum += properties.Limits.MaxDescriptorSetInputAttachments;
            sum += properties.Limits.MaxVertexInputAttributes;
            sum += properties.Limits.MaxVertexInputBindings;
            sum += properties.Limits.MaxVertexInputAttributeOffset;
            sum += properties.Limits.MaxVertexInputBindingStride;
            sum += properties.Limits.MaxVertexOutputComponents;
            sum += properties.Limits.MaxTessellationGenerationLevel;
            sum += properties.Limits.MaxTessellationPatchSize;
            sum += properties.Limits.MaxTessellationControlPerVertexInputComponents;
            sum += properties.Limits.MaxTessellationControlPerVertexOutputComponents;
            sum += properties.Limits.MaxTessellationControlPerPatchOutputComponents;
            sum += properties.Limits.MaxTessellationControlTotalOutputComponents;
            sum += properties.Limits.MaxTessellationEvaluationInputComponents;
            sum += properties.Limits.MaxTessellationEvaluationOutputComponents;
            sum += properties.Limits.MaxGeometryShaderInvocations;
            sum += properties.Limits.MaxGeometryInputComponents;
            sum += properties.Limits.MaxGeometryOutputComponents;
            sum += properties.Limits.MaxGeometryOutputVertices;
            sum += properties.Limits.MaxGeometryTotalOutputComponents;
            sum += properties.Limits.MaxFragmentInputComponents;
            sum += properties.Limits.MaxFragmentOutputAttachments;
            sum += properties.Limits.MaxFragmentDualSrcAttachments;
            sum += properties.Limits.MaxFragmentCombinedOutputResources;
            sum += properties.Limits.MaxComputeSharedMemorySize;
            sum += properties.Limits.MaxMemoryAllocationCount;
            sum += properties.Limits.MaxSamplerAllocationCount;
            
            return (int) sum;
        }

        return (settings.IsDeviceSuitable(properties, features));
    }
}