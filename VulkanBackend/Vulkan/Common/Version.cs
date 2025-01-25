namespace VulkanBackend.Vulkan.Common;

public class VulkanVersion
{
    public uint Major { get; }
    public uint Minor { get; }
    public uint Patch { get; }
    
    public VulkanVersion(uint major, uint minor, uint patch)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
    }
    
    public override string ToString()
    {
        return $"v{Major}.{Minor}.{Patch}";
    }
    
    public static bool operator >(VulkanVersion a, VulkanVersion b)
    {
        if (a.Major > b.Major)
            return true;
        if (a.Major == b.Major && a.Minor > b.Minor)
            return true;
        if (a.Major == b.Major && a.Minor == b.Minor && a.Patch > b.Patch)
            return true;
        return false;
    }
    
    public static bool operator <(VulkanVersion a, VulkanVersion b)
    {
        if (a.Major < b.Major)
            return true;
        if (a.Major == b.Major && a.Minor < b.Minor)
            return true;
        if (a.Major == b.Major && a.Minor == b.Minor && a.Patch < b.Patch)
            return true;
        return false;
    }
    
    public static VulkanVersion FromUInt(uint version)
    {
        uint major = version >> 22;
        uint minor = (version >> 12) & 0x3FF;
        uint patch = version & 0xFFF;
        return new VulkanVersion(major, minor, patch);
    }
    
    public static uint ToUInt(VulkanVersion version)
    {
        return (version.Major << 22) | (version.Minor << 12) | version.Patch;
    }
    
    public uint ToUInt()
    {
        return ToUInt(this);
    }
    
    // Default cast to uint
    public static implicit operator uint(VulkanVersion version)
    {
        return version.ToUInt();
    }
}