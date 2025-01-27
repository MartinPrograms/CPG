using System.Runtime.InteropServices;
using CPG;
using Silk.NET.Vulkan;

namespace VulkanBackend.Vulkan.Common;

public class DebugMessenger
{
    public unsafe static DebugUtilsMessengerCallbackFunctionEXT DebugMessengerCallback = DebugMessengerCallbackImpl;

    public static bool CrashOnError = true;
    public static DebugUtilsMessageSeverityFlagsEXT MinimumPrintSeverity = DebugUtilsMessageSeverityFlagsEXT.InfoBitExt;
    
    private static unsafe uint DebugMessengerCallbackImpl(DebugUtilsMessageSeverityFlagsEXT messageseverity, DebugUtilsMessageTypeFlagsEXT messagetypes, DebugUtilsMessengerCallbackDataEXT* pcallbackdata, void* puserdata)
    {
        if (messageseverity == DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt)
        {
            if (CrashOnError)
            {
                Logger.Fatal("Vulkan Error: " + Marshal.PtrToStringAnsi((IntPtr)pcallbackdata->PMessage), "VulkanDebugMessenger");
                throw new Exception("Vulkan Error: " + Marshal.PtrToStringAnsi((IntPtr)pcallbackdata->PMessage));
            }
            
            Logger.Error("Vulkan Error: " + Marshal.PtrToStringAnsi((IntPtr)pcallbackdata->PMessage), "VulkanDebugMessenger");
        }
        
        if (messageseverity == DebugUtilsMessageSeverityFlagsEXT.WarningBitExt)
        {
            Logger.Warning("Vulkan Warning: " + Marshal.PtrToStringAnsi((IntPtr)pcallbackdata->PMessage), "VulkanDebugMessenger");
        }
        
        if (messageseverity == DebugUtilsMessageSeverityFlagsEXT.InfoBitExt)
        {
            Logger.Info("Vulkan Info: " + Marshal.PtrToStringAnsi((IntPtr)pcallbackdata->PMessage), "VulkanDebugMessenger");
        }
        
        if (messageseverity == DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt)
        {
            Logger.Info("Vulkan Verbose: " + Marshal.PtrToStringAnsi((IntPtr)pcallbackdata->PMessage), "VulkanDebugMessenger");
        }
        
        if (MinimumPrintSeverity > messageseverity)
        {
            Console.WriteLine($"[Vulkan {messageseverity.ToString()}] {Marshal.PtrToStringAnsi((IntPtr)pcallbackdata->PMessage)}");
        }
        
        return 0;
    }

}