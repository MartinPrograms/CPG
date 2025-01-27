using CPG;
using CPG.Common;
using Silk.NET.Vulkan;
using VulkanBackend.Settings;
using VulkanBackend.Vulkan.Common;
using VulkanBackend.Vulkan.Extensions;

namespace VulkanBackend.Vulkan.Initialization.Helpers;

public class InstanceHelper
{
    public unsafe static Instance CreateInstance(InitializationSettings settings, WindowVK window,
        out DebugUtilsMessengerEXT messengerExt)
    {
        Logger.Info("Creating Vulkan Instance", "VulkanBackend");

        var appCreateInfo = new ApplicationInfo
        {
            SType = StructureType.ApplicationInfo,
            PApplicationName = settings.ApplicationName.ToPointer(),
            ApplicationVersion = settings.ApplicationVersion,
            PEngineName = settings.EngineName.ToPointer(),
            EngineVersion = settings.EngineVersion,
            ApiVersion = settings.ApiVersion
        };
     
        // List out the application info
        Logger.Info("Application Info", "VulkanBackend");
        Logger.Info($"Application Name: {settings.ApplicationName}", "VulkanBackend");
        Logger.Info($"Application Version: {settings.ApplicationVersion}", "VulkanBackend");
        Logger.Info($"Engine Name: {settings.EngineName}", "VulkanBackend");
        Logger.Info($"Engine Version: {settings.EngineVersion}", "VulkanBackend");
        Logger.Info($"API Version: {settings.ApiVersion}", "VulkanBackend");

        List<string> layers = settings.EnabledLayers;
        List<string> extensions = settings.EnabledExtensions;
        
        if (settings.ValidationLayers)
        {
            layers.Add("VK_LAYER_KHRONOS_validation");
        }

        if (settings.Debug)
        {
            extensions.Add("VK_EXT_debug_utils");
        }
        
        var windowExtensions = window.GetRequiredExtensions();
        extensions.AddRange(windowExtensions);
        
        // List out the layers and extensions
        Logger.Info("Layers", "VulkanBackend");
        foreach (var layer in layers)
        {
            Logger.Info(layer, "VulkanBackend");
        }
        
        Logger.Info("Extensions", "VulkanBackend");
        foreach (var extension in extensions)
        {
            Logger.Info(extension, "VulkanBackend");
        }
        
        var instanceCreateInfo = new InstanceCreateInfo
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appCreateInfo,
            EnabledLayerCount = (uint)layers.Count,
            PpEnabledLayerNames = layers.ToPointerArray(),
            EnabledExtensionCount = (uint)extensions.Count,
            PpEnabledExtensionNames = extensions.ToPointerArray()
        };
        
        // Create the instance
        var vk = Context.Current.Vk;
        vk.EnsureSuccess(vk.CreateInstance(&instanceCreateInfo, null, out var instance));

        if (settings.Debug)
        {
            var debugCreateInfo = new DebugUtilsMessengerCreateInfoEXT
            {
                SType = StructureType.DebugUtilsMessengerCreateInfoExt,
                MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.InfoBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.WarningBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt,
                MessageType = DebugUtilsMessageTypeFlagsEXT.ValidationBitExt |
                              DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt |
                              DebugUtilsMessageTypeFlagsEXT.GeneralBitExt,
                PfnUserCallback = DebugMessenger.DebugMessengerCallback,
                PUserData = null
            };

            if (!vk.TryGetInstanceExtension<Silk.NET.Vulkan.Extensions.EXT.ExtDebugUtils>(instance,
                    out var debugUtilsExtension))
            {
                throw new Exception("Failed to get debug utils extension");
            }

            if (debugUtilsExtension.CreateDebugUtilsMessenger(instance, &debugCreateInfo, null, out var messenger) !=
                Result.Success)
            {
                throw new Exception("Failed to create debug messenger");
            }

            messengerExt = messenger;
        }
        else
        {
            messengerExt = default;
        }
        
        Logger.Info("Vulkan Instance Created", "VulkanBackend");
        
        return instance;
    }

    public static unsafe void DestroyInstance(Instance objInstance, DebugUtilsMessengerEXT objDebugUtils)
    {
        var vk = Context.Current.Vk;
        
        if (objDebugUtils.Handle != 0)
        {
            var hasExt = vk.TryGetInstanceExtension<Silk.NET.Vulkan.Extensions.EXT.ExtDebugUtils>(objInstance,
                out var debugUtilsExtension);
            if (hasExt)
            {
                debugUtilsExtension.DestroyDebugUtilsMessenger(objInstance, objDebugUtils, null);
            }
        }

        vk.DestroyInstance(objInstance, null);
    }
}